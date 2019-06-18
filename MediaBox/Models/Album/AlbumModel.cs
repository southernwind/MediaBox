using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Livet;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Gesture;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {

	/// <summary>
	/// アルバムクラス
	/// </summary>
	/// <remarks>
	/// 複数の<see cref="IMediaFileModel"/>を保持、管理するクラス。
	/// <see cref="MediaFileCollection.Items"/>に持っている
	/// </remarks>
	internal abstract class AlbumModel : MediaFileCollection, IAlbumModel {
		private readonly CancellationTokenSource _loadFullSizeImageCts;
		private CancellationTokenSource _loadMediaFilesCts;
		private readonly object _loadMediaFilesCtsLockObject = new object();

		private readonly IAlbumSelector _selector;
		private bool _isScalingMode = false;

		private readonly ObservableSynchronizedCollection<PriorityWith<IMediaFileModel>> _loadingImages = new ObservableSynchronizedCollection<PriorityWith<IMediaFileModel>>();
		private readonly ContinuousTaskAction _taskAction;
		protected readonly PriorityTaskQueue PriorityTaskQueue;

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
		/// マップ
		/// </summary>
		public IReactiveProperty<MapModel> Map {
			get;
		}

		/// <summary>
		/// カレントインデックス番号
		/// </summary>
		public IReactiveProperty<int> CurrentIndex {
			get;
		} = new ReactivePropertySlim<int>(-1, mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);

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
		/// カレントのメディアファイルの情報
		/// </summary>
		public IReadOnlyReactiveProperty<MediaFileInformation> MediaFileInformation {
			get;
		}

		/// <summary>
		/// 表示モード
		/// </summary>
		public IReactiveProperty<DisplayMode> DisplayMode {
			get;
		}

		/// <summary>
		/// 一覧ズームレベル
		/// </summary>
		public IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		/// <summary>
		/// 操作受信
		/// </summary>
		public GestureReceiver GestureReceiver {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="items">このインスタンスで利用するメディアファイルリスト</param>
		/// <param name="selector">このクラスを保有しているアルバムセレクター</param>
		protected AlbumModel(ObservableSynchronizedCollection<IMediaFileModel> items, IAlbumSelector selector) : base(items) {
			this.GestureReceiver = Get.Instance<GestureReceiver>();
			this._loadFullSizeImageCts = new CancellationTokenSource().AddTo(this.CompositeDisposable);
			this._selector = selector;
			this.MediaFileInformation =
				new ReactivePropertySlim<MediaFileInformation>(
					Get.Instance<MediaFileInformation>(selector)
				).ToReadOnlyReactivePropertySlim();

			this.PriorityTaskQueue = Get.Instance<PriorityTaskQueue>();
			this.ZoomLevel = this.Settings.GeneralSettings.ZoomLevel.ToReadOnlyReactivePropertySlim();

			// フルイメージロード用タスク
			this._taskAction = new ContinuousTaskAction(
				$"フルサイズイメージ読み込み[{this._loadingImages.Count}]",
				async state => await Task.Run(() => {
					while (true) {
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
				this._loadFullSizeImageCts.Token
			).AddTo(this.CompositeDisposable);
			this.PriorityTaskQueue.AddTask(this._taskAction);

			this.DisplayMode =
				this.Settings
					.GeneralSettings
					.DisplayMode
					.ToReactivePropertyAsSynchronized(x => x.Value)
					.AddTo(this.CompositeDisposable);

			// アイテム→マップアイテム片方向同期
			this.Map = new ReactivePropertySlim<MapModel>(Get.Instance<MapModel>(this.Items));

			this.Map.Value.OnSelect.Select(x => x.ToArray()).Subscribe(x => {
				this.CurrentMediaFiles.Value =
					x.All(this.CurrentMediaFiles.Value.Contains) ?
						this.CurrentMediaFiles.Value.Except(x) :
						this.CurrentMediaFiles.Value.Union(x.Where(m => !this.CurrentMediaFiles.Value.Contains(m))).ToList();
			});

			// カレントアイテム→プロパティ,マップカレントアイテム片方向同期
			this.CurrentMediaFiles.Select(x => x.ToArray()).Subscribe(x => {
				this.Map.Value.CurrentMediaFiles.Value = x;
				this.MediaFileInformation.Value.Files.Value = x;
			});

			// カレントアイテム→マップカレントアイテム同期
			this.CurrentIndex
				.Subscribe(x => {
					if (x >= 0 && this.Items.Count > x) {
						this.CurrentMediaFile.Value = this.Items[x];
					} else if (this.Items.Any() && x < 0) {
						this.CurrentIndex.Value = 0;
					} else {
						this.CurrentMediaFile.Value = null;
					}
					this.Map.Value.CurrentMediaFile.Value = this.CurrentMediaFile.Value;
					if (this.CurrentMediaFile.Value != null) {
						this.CurrentMediaFiles.Value = new[] { this.CurrentMediaFile.Value };
					}
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
						if (this.Count.Value != 0) {
							this.CurrentIndex.Value = 0;
						} else {
							this.CurrentIndex.Value = -1;
						}
					}
				});

			// 先読みロード
			this.CurrentIndex
				.CombineLatest(
					this.DisplayMode,
					(currentIndex, displayMode) => (currentIndex, displayMode))
				.Where(x => x.currentIndex >= 0)
				// TODO : 時間で制御はあまりやりたくないな　何か考える
				.Throttle(TimeSpan.FromMilliseconds(100))
				.Subscribe(x => {
					if (x.displayMode != Composition.Enum.DisplayMode.Detail) {
						// 全アンロード
						this.Prefetch(Array.Empty<IMediaFileModel>());
						return;
					}

					var minIndex = Math.Max(0, x.currentIndex - 2);
					IEnumerable<IMediaFileModel> models;
					lock (this.Items) {
						var count = Math.Min(x.currentIndex + 2, this.Items.Count - 1) - minIndex + 1;
						// 読み込みたい順に並べる
						models =
							Enumerable
								.Range(minIndex, count)
								.OrderBy(i => i >= x.currentIndex ? 0 : 1)
								.ThenBy(i => Math.Abs(i - x.currentIndex))
								.Select(i => this.Items[i])
								.ToArray();
					}
					this.Prefetch(models);
				});

			void selectPreviewItem() {
				var index = this.CurrentIndex.Value;
				if (index <= 0 && this.Items.Count != 0) {
					return;
				}
				this.CurrentMediaFiles.Value = new[] { this.Items[index - 1] };
			}

			void selectNextItem() {
				var index = this.CurrentIndex.Value;
				if (index + 1 >= this.Items.Count) {
					return;
				}
				this.CurrentMediaFiles.Value = new[] { this.Items[index + 1] };
			}

			this.GestureReceiver
				.KeyEvent
				.Subscribe(x => {
					switch (x.Key) {
						case Key.Left:
							if (x.IsDown) {
								selectPreviewItem();
							}
							break;
						case Key.Right:
							if (x.IsDown) {
								selectNextItem();
							}
							break;
						case Key.LeftCtrl:
						case Key.RightCtrl:
							this._isScalingMode = x.IsDown;
							break;

					}
				}).AddTo(this.CompositeDisposable);

			this.GestureReceiver
				.MouseWheelEvent
				.Subscribe(x => {
					if (this._isScalingMode) {
						if (this.DisplayMode.Value != Composition.Enum.DisplayMode.Library) {
							x.Handled = true;
							return;
						}
						if (x.Delta > 0) {
							if (this.ZoomLevel.Value <= Controls.Converters.ZoomLevel.MinLevel) {
								x.Handled = true;
								return;
							}
							this.Settings.GeneralSettings.ZoomLevel.Value -= 1;
						} else {
							if (this.ZoomLevel.Value >= Controls.Converters.ZoomLevel.MaxLevel) {
								x.Handled = true;
								return;
							}
							this.Settings.GeneralSettings.ZoomLevel.Value += 1;
						}
						x.Handled = true;
					} else {
						if (this.DisplayMode.Value != Composition.Enum.DisplayMode.Detail) {
							return;
						}
						if (x.Delta > 0) {
							selectPreviewItem();
						} else {
							selectNextItem();
						}
					}
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// フィルタリング前件数更新
		/// </summary>
		protected void UpdateBeforeFilteringCount() {
			lock (this.DataBase) {
				this.BeforeFilteringCount.Value = this.DataBase.MediaFiles.Count(this.WherePredicate());
			}
		}

		/// <summary>
		/// メディアファイルリスト読み込み
		/// </summary>
		public void LoadMediaFiles() {
			this.PriorityTaskQueue.AddTask(new TaskAction(
				"アルバム読み込み",
				async state => await Task.Run(() => {
					this._loadMediaFilesCts?.Cancel();
					lock (this._loadMediaFilesCtsLockObject) {
						this._loadMediaFilesCts = new CancellationTokenSource();

						MediaFile[] items;
						lock (this.DataBase) {
							this.UpdateBeforeFilteringCount();
							items = this
								._selector
								.FilterSetter
								.SetFilterConditions(
									this.DataBase
										.MediaFiles
										.Where(this.WherePredicate())
								)
							.Include(mf => mf.MediaFileTags)
							.ThenInclude(mft => mft.Tag)
							.Include(mf => mf.ImageFile)
							.Include(mf => mf.VideoFile)
							.Include(mf => mf.Position)
							.ToArray();
						}

						var mediaFiles = new IMediaFileModel[items.Length];
						foreach (var (item, index) in items.Select((x, i) => (x, i))) {
							if (this._loadMediaFilesCts.IsCancellationRequested) {
								return;
							}
							var m = this.MediaFactory.Create(item.FilePath);
							if (!m.FileInfoLoaded) {
								m.LoadFromDataBase(item);
								m.UpdateFileInfo();
							}
							mediaFiles[index] = m;
						}

						this.ItemsReset(this._selector.SortSetter.SetSortConditions(mediaFiles));
					}
				}), Priority.LoadMediaFiles, CancellationToken.None));


		}

		/// <summary>
		/// 表示モードの変更を行う。
		/// </summary>
		/// <param name="displayMode">変更後表示モード</param>
		public void ChangeDisplayMode(DisplayMode displayMode) {
			this.Settings.GeneralSettings.DisplayMode.Value = displayMode;
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
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected abstract Expression<Func<MediaFile, bool>> WherePredicate();

		/// <summary>
		/// Dispose
		/// </summary>
		/// <remarks>
		/// 実行中の非同期処理をキャンセルしてからDisposeする。
		/// </remarks>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing) {
			if (this.Disposed) {
				return;
			}
			this._loadFullSizeImageCts.Cancel();
			this._loadMediaFilesCts?.Cancel();
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
