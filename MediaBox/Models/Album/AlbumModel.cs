using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Library.EventAsObservable;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバムクラス
	/// </summary>
	internal abstract class AlbumModel : MediaFileCollection {
		private readonly CancellationTokenSource _cancellationTokenSource;
		/// <summary>
		/// キャンセルトークン Dispose時にキャンセルされる。
		/// </summary>
		protected CancellationToken CancellationToken {
			get {
				return this._cancellationTokenSource.Token;
			}
		}

		// TODO : テスト以外使ってない。なんとか考えて削除する。
#pragma warning disable IDE0052 // Remove unread private members
		/// <summary>
		/// Dispose前のTask待機用にメンバ変数に確保
		/// </summary>
		private readonly ReadOnlyReactiveCollection<Fsw> _fileSystemWatchers;
#pragma warning restore IDE0052 // Remove unread private members

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// ファイル更新監視ディレクトリ
		/// </summary>
		public ReactiveCollection<string> MonitoringDirectories {
			get;
		} = new ReactiveCollection<string>();

		/// <summary>
		/// マップ
		/// </summary>
		public IReactiveProperty<MapModel> Map {
			get;
		} = new ReactivePropertySlim<MapModel>(Get.Instance<MapModel>());

		/// <summary>
		/// カレントのメディアファイル(単一)
		/// </summary>
		public IReactiveProperty<MediaFileModel> CurrentMediaFile {
			get;
		} = new ReactivePropertySlim<MediaFileModel>();

		/// <summary>
		/// カレントのメディアファイル(複数)
		/// </summary>
		public IReactiveProperty<IEnumerable<MediaFileModel>> CurrentMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<MediaFileModel>>(Array.Empty<MediaFileModel>());

		/// <summary>
		/// カレントのメディアファイルのプロパティ
		/// </summary>
		public IReactiveProperty<MediaFileInformations> MediaFileProperties {
			get;
		} = new ReactivePropertySlim<MediaFileInformations>(Get.Instance<MediaFileInformations>());

		/// <summary>
		/// 表示モード
		/// </summary>
		public IReactiveProperty<DisplayMode> DisplayMode {
			get;
		}

		protected AlbumModel() {
			this._cancellationTokenSource = new CancellationTokenSource();
			this._cancellationTokenSource.AddTo(this.CompositeDisposable);

			this.DisplayMode =
				this.Settings
					.GeneralSettings
					.DisplayMode
					.ToReactivePropertyAsSynchronized(x => x.Value)
					.AddTo(this.CompositeDisposable);

			this.Items
				.ToCollectionChanged()
				.Subscribe(x => {
					switch (x.Action) {
						case NotifyCollectionChangedAction.Add:
							this.OnAddedItem(x.Value);
							break;
						case NotifyCollectionChangedAction.Remove:
							this.OnRemovedItem(x.Value);
							break;
					}
				}).AddTo(this.CompositeDisposable);

			// アイテム→マップアイテム片方向同期
			this.Items.SynchronizeTo(this.Map.Value.Items).AddTo(this.CompositeDisposable);

			this.Map.Value.OnSelect.Select(x => x.ToArray()).Subscribe(x => {
				this.CurrentMediaFiles.Value =
					x.All(this.CurrentMediaFiles.Value.Contains) ?
						this.CurrentMediaFiles.Value.Except(x) :
						this.CurrentMediaFiles.Value.Union(x.Where(m => !this.CurrentMediaFiles.Value.Contains(m))).ToList();
			});

			// カレントアイテム→プロパティ,マップカレントアイテム片方向同期
			this.CurrentMediaFiles.Select(x => x.ToArray()).Subscribe(x => {
				this.Map.Value.CurrentMediaFiles.Value = x;
				this.MediaFileProperties.Value.Files.Value = x;
			});

			// カレントアイテムの先頭を取得
			this.CurrentMediaFiles
				.Subscribe(_ => {
					this.CurrentMediaFile.Value = this.CurrentMediaFiles.Value.FirstOrDefault();
				}).AddTo(this.CompositeDisposable);

			// カレントを外れたメディアのフルイメージアンロード
			this.CurrentMediaFile
				.Pairwise()
				.Subscribe(x => {
					if (x.OldItem is ImageFileModel ifm) {
						ifm.UnloadImage();
					}
				}).AddTo(this.CompositeDisposable);

			// カレントメディアフルイメージロード
			this.CurrentMediaFile
				.CombineLatest(
					this.DisplayMode,
					(currentItem, displayMode) => (currentItem, displayMode))
				.Where(x => x.displayMode == Composition.Enum.DisplayMode.Detail)
				.Where(x => x.currentItem != null)
				// TODO : 時間で制御はあまりやりたくないな　何か考える
				.Throttle(TimeSpan.FromMilliseconds(30))
				.ObserveOnBackground(this.Settings.ForTestSettings.RunOnBackground.Value)
				.Subscribe(async x => {
					if (x.currentItem is ImageFileModel ifm) {
						await ifm.LoadImageAsync();
					}
				});

			// カレントアイテム→マップカレントアイテム同期
			this.CurrentMediaFile
				.Subscribe(x => {
					this.Map.Value.CurrentMediaFile.Value = x;
				}).AddTo(this.CompositeDisposable);


			// ファイル情報読み込み
			this.CurrentMediaFile
				.Where(x => x != null)
				.Subscribe(x => {
					x.GetFileInfoIfNotLoaded();
				}).AddTo(this.CompositeDisposable);

			// ファイル更新監視
			// FswクラスにFileSystemWatcherと、初期読み込みのTaskと、Taskをキャンセルするキャンセルトークンをもたせておき、Dispose時に利用する
			this._fileSystemWatchers = this.MonitoringDirectories
				.ToReadOnlyReactiveCollection(md => {
					if (!Directory.Exists(md)) {
						this.Logging.Log($"監視フォルダが見つかりません。{md}", LogLevel.Warning);
						return null;
					}
					// TODO : Taskを投げっぱなしなのと、FileSystemWatcherの監視開始が読み込み完了前な点が気になる
					var fsw = new Fsw {
						TokenSource = new CancellationTokenSource(),
						FileSystemWatcher = new FileSystemWatcher(md) {
							IncludeSubdirectories = true,
							EnableRaisingEvents = true
						}
					};
					var disposable = Observable.Merge(
						fsw.FileSystemWatcher.CreatedAsObservable(),
						fsw.FileSystemWatcher.RenamedAsObservable(),
						fsw.FileSystemWatcher.ChangedAsObservable(),
						fsw.FileSystemWatcher.DeletedAsObservable()
						).Subscribe(this.OnFileSystemEvent);

					fsw.FileSystemWatcher.DisposedAsObservable().Subscribe(_ => disposable.Dispose());
					fsw.Task = Task.Run(async () => {
						await Observable
							.Start(() => {
								this.Load(md, fsw.TokenSource.Token);
							})
							.ObserveOnBackground(this.Settings.ForTestSettings.RunOnBackground.Value)
							.FirstAsync();
					});

					return fsw;
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">フォルダパス</param>
		protected abstract void LoadFileInDirectory(string directoryPath);

		/// <summary>
		/// リストにメディアファイルが追加されたときに呼ばれる。
		/// </summary>
		/// <param name="mediaFile">追加されたメディアファイル</param>
		protected virtual void OnAddedItem(MediaFileModel mediaFile) {

		}

		protected virtual void OnRemovedItem(MediaFileModel mediaFile) {

		}

		protected virtual void OnFileSystemEvent(FileSystemEventArgs e) {
			if (!e.FullPath.IsTargetExtension()) {
				return;
			}

			switch (e.ChangeType) {
				case WatcherChangeTypes.Created:
					this.Items.Add(this.MediaFactory.Create(e.FullPath));
					break;
				case WatcherChangeTypes.Deleted:
					this.Items.Remove(this.Items.Single(i => i.FilePath == e.FullPath));
					break;
			}
		}

		public void ChangeDisplayMode(DisplayMode displayMode) {
			this.Settings.GeneralSettings.DisplayMode.Value = displayMode;
		}

		protected override void Dispose(bool disposing) {
			if (this.Disposed) {
				return;
			}
			this._cancellationTokenSource.Cancel();
			base.Dispose(disposing);
		}

		private void Load(string path, CancellationToken token) {
			if (token.IsCancellationRequested) {
				return;
			}
			if (!Directory.Exists(path)) {
				return;
			}
			try {
				this.LoadFileInDirectory(path);
			} catch (UnauthorizedAccessException) {
			}
			foreach (var dir in Directory.EnumerateDirectories(path)) {
				try {
					this.Load(dir, token);
				} catch (UnauthorizedAccessException) {

				}
			}
		}


		internal class Fsw : IDisposable {
			public CancellationTokenSource TokenSource {
				get;
				set;
			}

			public FileSystemWatcher FileSystemWatcher {
				get;
				set;
			}

			public Task Task {
				get;
				set;
			}

			public void Dispose() {
				this.TokenSource?.Cancel();
				this.FileSystemWatcher?.Dispose();
				this.TokenSource?.Dispose();
			}
		}
	}
}
