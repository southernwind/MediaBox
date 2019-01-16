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
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.EventAsObservable;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバムクラス
	/// </summary>
	internal abstract class Album : MediaFileCollection {
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
		public ReactivePropertySlim<MapModel> Map {
			get;
		} = new ReactivePropertySlim<MapModel>(Get.Instance<MapModel>());

		/// <summary>
		/// カレントのメディアファイル(単一)
		/// </summary>
		public ReactivePropertySlim<MediaFile> CurrentMediaFile {
			get;
		} = new ReactivePropertySlim<MediaFile>();

		/// <summary>
		/// カレントのメディアファイル(複数)
		/// </summary>
		public ReactivePropertySlim<IEnumerable<MediaFile>> CurrentMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<MediaFile>>(Array.Empty<MediaFile>());

		/// <summary>
		/// カレントのメディアファイルのプロパティ
		/// </summary>
		public ReactivePropertySlim<MediaFileProperties> MediaFileProperties {
			get;
		} = new ReactivePropertySlim<MediaFileProperties>(Get.Instance<MediaFileProperties>());

		/// <summary>
		/// 表示モード
		/// </summary>
		public ReactiveProperty<DisplayMode> DisplayMode {
			get;
		}

		/// <summary>
		/// 外部ツール
		/// </summary>
		public ReadOnlyReactiveCollection<ExternalTool> ExternalTools {
			get;
		}

		protected Album() {
			this._cancellationTokenSource = new CancellationTokenSource();
			this._cancellationTokenSource.AddTo(this.CompositeDisposable);

			this.DisplayMode =
				this.Settings
					.GeneralSettings
					.DisplayMode
					.ToReactivePropertyAsSynchronized(x => x.Value)
					.AddTo(this.CompositeDisposable);

			this.ExternalTools =
				this.Settings
					.GeneralSettings
					.ExternalTools
					.ToReadOnlyReactiveCollection()
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

			// カレントアイテムフルイメージロード
			this.CurrentMediaFile
				.Pairwise()
				.CombineLatest(
					this.DisplayMode,
					(currentItem, displayMode) => (currentItem, displayMode))
				.ObserveOnBackground(this.Settings.ForTestSettings.RunOnBackground.Value)
				.Subscribe(async x => {
					x.currentItem.OldItem?.UnloadImage();
					if (x.displayMode != Composition.Enum.DisplayMode.Detail) {
						return;
					}

					if (x.currentItem.NewItem == null) {
						return;
					}
					await x.currentItem.NewItem.LoadImageAsync();
				}).AddTo(this.CompositeDisposable);

			// カレントアイテム→マップカレントアイテム同期
			this.CurrentMediaFile
				.Subscribe(x => {
					this.Map.Value.CurrentMediaFile.Value = x;
				}).AddTo(this.CompositeDisposable);


			// Exifロード
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
		protected virtual void OnAddedItem(MediaFile mediaFile) {

		}

		protected virtual void OnRemovedItem(MediaFile mediaFile) {

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
