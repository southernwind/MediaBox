using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Loader;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Services;
using SandBeige.MediaBox.ViewModels;

namespace SandBeige.MediaBox.Models.Album {

	/// <summary>
	/// アルバムクラス
	/// </summary>
	/// <remarks>
	/// 複数の<see cref="IMediaFileModel"/>を保持、管理するクラス。
	/// <see cref="MediaFileCollection.Items"/>に持っている
	/// </remarks>
	public class AlbumModel : MediaFileCollection, IAlbumModel {
		private readonly CancellationTokenSource _loadFullSizeImageCts;
		private CancellationTokenSource _loadMediaFilesCts;

		private readonly ObservableSynchronizedCollection<PriorityWith<IMediaFileModel>> _loadingImages = new ObservableSynchronizedCollection<PriorityWith<IMediaFileModel>>();
		private readonly ContinuousTaskAction _taskAction;
		protected readonly PriorityTaskQueue PriorityTaskQueue;
		private readonly ViewModelFactory _viewModelFactory;
		private readonly AlbumViewerManager _albumViewerManager;
		private ReadOnlyReactiveCollection<IAlbumViewerViewViewModelPair> _albumViewer;
		private IReactiveProperty<IAlbumViewerViewViewModelPair> _currentAlbumViewer;
		private readonly AlbumLoaderFactory _albumLoaderFactory;
		private AlbumLoader _albumLoader;

		/// <summary>
		/// アルバムオブジェクト
		/// </summary>
		public IAlbumObject AlbumObject {
			get;
			private set;
		}

		/// <summary>
		/// フィルタリング前件数
		/// </summary>
		public IReactiveProperty<int> BeforeFilteringCount {
			get;
		} = new ReactivePropertySlim<int>();

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// アルバム読み込み時間(ms)
		/// </summary>
		public IReactiveProperty<long> ResponseTime {
			get;
		} = new ReactivePropertySlim<long>(-1);

		/// <summary>
		/// カレントのメディアファイル(単一)
		/// </summary>
		public IReactiveProperty<IMediaFileModel> CurrentMediaFile {
			get;
		} = new ReactivePropertySlim<IMediaFileModel>();

		/// <summary>
		/// カレントのメディアファイル(複数)
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileModel>> CurrentMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>(Array.Empty<IMediaFileModel>());

		/// <summary>
		/// 一覧ズームレベル
		/// </summary>
		public IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		/// <summary>
		/// 操作受信
		/// </summary>
		public IGestureReceiver GestureReceiver {
			get;
		}

		public ReadOnlyReactiveCollection<IAlbumViewerViewViewModelPair> AlbumViewers {
			get {
				return this._albumViewer ??= this._albumViewerManager
						.AlbumViewerList
						.ToReadOnlyReactiveCollection(x => x.Create(this._viewModelFactory.Create(this)))
						.AddTo(this.CompositeDisposable);
			}
		}

		public IReactiveProperty<IAlbumViewerViewViewModelPair> CurrentAlbumViewer {
			get {
				return this._currentAlbumViewer ??= new ReactivePropertySlim<IAlbumViewerViewViewModelPair> { Value = this.AlbumViewers.First() };
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="items">このインスタンスで利用するメディアファイルリスト</param>
		public AlbumModel(
			ObservableSynchronizedCollection<IMediaFileModel> items,
			ISettings settings,
			IGestureReceiver gestureReceiver,
			ViewModelFactory viewModelFactory,
			PriorityTaskQueue priorityTaskQueue,
			AlbumViewerManager albumViewerManager,
			VolatilityStateShareService volatilityStateShareService,
			AlbumLoaderFactory albumLoaderFactory) : base(items) {
			this.GestureReceiver = gestureReceiver;
			this._loadFullSizeImageCts = new CancellationTokenSource().AddTo(this.CompositeDisposable);
			this._viewModelFactory = viewModelFactory;
			this._albumViewerManager = albumViewerManager;
			this._albumLoaderFactory = albumLoaderFactory;

			this.PriorityTaskQueue = priorityTaskQueue;
			this.ZoomLevel = settings.GeneralSettings.ZoomLevel.ToReadOnlyReactivePropertySlim();

			// フルイメージロード用タスク
			this._taskAction = new ContinuousTaskAction(
				$"フルサイズイメージ読み込み[{this._loadingImages.Count}]",
				async state => await Task.Run(() => {
					while (true) {
						if (state.CancellationToken.IsCancellationRequested) {
							return;
						}
						var m = this._loadingImages.Where(x => !x.Completed).OrderBy(p => p.Priority).FirstOrDefault();
						if (m == null) {
							return;
						}
						if (m.Object is ImageFileModel ifm) {
							ifm.LoadImageIfNotLoaded();
						}
						m.Completed = true;
					}
				}),
				Priority.LoadFullImage,
				this._loadFullSizeImageCts
			).AddTo(this.CompositeDisposable);
			this.PriorityTaskQueue.AddTask(this._taskAction);

			this.CurrentMediaFiles.Select(x => x.ToArray()).Subscribe(x => {
				volatilityStateShareService.MediaFileModels.Value = x;
				// 代表ファイルの設定
				this.CurrentMediaFile.Value = x.FirstOrDefault();
			}).AddTo(this.CompositeDisposable);

			this.Items
				.CollectionChangedAsObservable()
				.Where(x =>
					new[] {
						NotifyCollectionChangedAction.Remove ,
						NotifyCollectionChangedAction.Replace,
						NotifyCollectionChangedAction.Reset
					}.Contains(x.Action))
				.Subscribe(x => {
					if (!this.Items.Contains(this.CurrentMediaFile.Value)) {
						if (x.OldStartingIndex == -1) {
							this.CurrentMediaFiles.Value = this.Items.Take(1);
						} else {
							this.CurrentMediaFiles.Value = new[] { this.Items[x.OldStartingIndex] };
						}
					}
				});

			this.ZoomLevel = this.GestureReceiver
				.MouseWheelEvent
				.Where(_ => this.GestureReceiver.IsControlKeyPressed)
				.ToZoomLevel(settings.GeneralSettings.ZoomLevel)
				.AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// アルバム切り替え
		/// </summary>
		public void SetAlbum(IAlbumObject albumObject) {
			lock (this._albumLoader ?? new object()) {
				this._albumLoader?.Dispose();
				this._albumLoader = this._albumLoaderFactory.Create(albumObject);
				this._albumLoader.OnAlbumDefinitionUpdated.Subscribe(_ => {
					this.UpdateBeforeFilteringCount();
					this.LoadMediaFiles();
				}).AddTo(this.CompositeDisposable);

				this._albumLoader.OnDeleteFile.Subscribe(x => {
					this.UpdateBeforeFilteringCount();
					lock (this.Items) {
						this.Items.RemoveRange(x);
					}
				});
				this._albumLoader.OnAddFile.Subscribe(x => {
					this.UpdateBeforeFilteringCount();
					lock (this.Items) {
						this.Items.AddRange(x);
					}
				});
			}
			this.UpdateBeforeFilteringCount();
			this.LoadMediaFiles();
		}

		public void SelectPreviewItem() {
			var index = this.Items.IndexOf(this.CurrentMediaFile.Value);
			if (index <= 0 && this.Items.Count != 0) {
				return;
			}
			this.CurrentMediaFiles.Value = new[] { this.Items[index - 1] };
		}

		public void SelectNextItem() {
			var index = this.Items.IndexOf(this.CurrentMediaFile.Value);
			if (index + 1 >= this.Items.Count) {
				return;
			}
			this.CurrentMediaFiles.Value = new[] { this.Items[index + 1] };
		}


		/// <summary>
		/// フィルタリング前件数更新
		/// </summary>
		protected void UpdateBeforeFilteringCount() {
			this.BeforeFilteringCount.Value = this._albumLoader.GetBeforeFilteringCount();
		}

		/// <summary>
		/// メディアファイルリスト読み込み
		/// </summary>
		public void LoadMediaFiles() {
			lock (this._loadMediaFilesCts ?? new object()) {
				this._loadMediaFilesCts?.Dispose();
				this._loadMediaFilesCts = new CancellationTokenSource();
				this.PriorityTaskQueue.AddTask(new TaskAction(
					"アルバム読み込み",
					async state => {
						var sw = new Stopwatch();
						sw.Start();
						this.CurrentMediaFiles.Value = await this._albumLoader.LoadMediaFiles(state);
						sw.Stop();
						this.ResponseTime.Value = sw.ElapsedMilliseconds;
					}, Priority.LoadMediaFiles, this._loadMediaFilesCts));
			}
		}

		/// <summary>
		/// 事前読み込み
		/// </summary>
		/// <remarks>
		/// 事前読み込みしたい画像リストを受け取って受け取った順番どおりに事前読み込みを行う
		/// Remove
		/// </remarks>
		/// <param name="models">事前読み込みが必要なメディアリスト</param>
		public void Prefetch(IEnumerable<IMediaFileModel> models) {
			// 　↓要件
			// ・前回の読み込みの途中で、かつ今回も読み込みリストに入っている場合だった場合、そのまま読み込み継続させる
			// ・読み込みリストから消えた場合はフルイメージをアンロードする
			// ・読み込みは渡されたコレクションの順番の通りに行う

			// いなくなった分は削除
			var removeList = this._loadingImages.Where(li => !models.Contains(li.Object)).ToArray();
			foreach (var item in removeList) {
				this._loadingImages.Remove(item);
			}
			// 追加された分は追加
			foreach (var item in models.Except(this._loadingImages.Select(li => li.Object)).Select(li => new PriorityWith<IMediaFileModel>(li, 0))) {
				this._loadingImages.Add(item);
			}
			// 優先度を追加された順に変更
			foreach (var (model, index) in models.Select((x, i) => (x, i))) {
				var image = this._loadingImages.SingleOrDefault(x => x.Object == model);
				// 別スレッドで削除している可能性があるのでnull判定をしておく
				if (image == null) {
					continue;
				}
				image.Priority = index;
			}
			// いらなくなったイメージはアンロードしておく
			foreach (var m in removeList.Select(x => x)) {
				if (m.Object is ImageFileModel ifm) {
					ifm.UnloadImage();
				}
			}

			if (this._loadingImages.Count == 0) {
				return;
			}

			this._taskAction.Restart();
		}

		/// <summary>
		/// Dispose
		/// </summary>
		/// <remarks>
		/// 実行中の非同期処理をキャンセルしてからDisposeする。
		/// </remarks>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing) {
			using (this.DisposeLock.DisposableEnterWriteLock()) {
				if (this.DisposeState != DisposeState.NotDisposed) {
					return;
				}
				this._loadFullSizeImageCts.Cancel();
				this._loadMediaFilesCts?.Cancel();
			}
			base.Dispose(disposing);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}

		public class PriorityWith<T> {
			public T Object {
				get;
			}

			public int Priority {
				get;
				set;
			}

			public bool Completed {
				get;
				set;
			}

			public PriorityWith(T obj, int priority) {
				this.Object = obj;
				this.Priority = priority;
			}
		}
	}
}
