using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
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
	/// <remarks>
	/// 複数の<see cref="IMediaFileModel"/>を保持、管理するクラス。
	/// <see cref="MediaFileCollection.Items"/>に持っている
	/// </remarks>
	internal abstract class AlbumModel : MediaFileCollection {
		private readonly CancellationTokenSource _cancellationTokenSource;

		/// <summary>
		/// キャンセルトークン Dispose時にキャンセルされる。
		/// </summary>
		protected CancellationToken CancellationToken {
			get;
		}

		/// <summary>
		/// サムネイル作成場所
		/// </summary>
		protected ThumbnailLocation ThumbnailLocation {
			get;
			set;
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
		public IReactiveProperty<MediaFileInformations> MediaFileInformations {
			get;
		} = new ReactivePropertySlim<MediaFileInformations>(Get.Instance<MediaFileInformations>());

		/// <summary>
		/// 表示モード
		/// </summary>
		public IReactiveProperty<DisplayMode> DisplayMode {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected AlbumModel() {
			this._cancellationTokenSource = new CancellationTokenSource();
			this._cancellationTokenSource.AddTo(this.CompositeDisposable);
			this.CancellationToken = this._cancellationTokenSource.Token;

			this.DisplayMode =
				this.Settings
					.GeneralSettings
					.DisplayMode
					.ToReactivePropertyAsSynchronized(x => x.Value)
					.AddTo(this.CompositeDisposable);

			// アイテム→マップアイテム片方向同期
			this.Items
				.SynchronizeTo<IMediaFileModel, ObservableSynchronizedCollection<IMediaFileModel>, ObservableSynchronizedCollection<IMediaFileModel>>(this.Map.Value.Items)
				.AddTo(this.CompositeDisposable);

			this.Map.Value.OnSelect.Select(x => x.ToArray()).Subscribe(x => {
				this.CurrentMediaFiles.Value =
					x.All(this.CurrentMediaFiles.Value.Contains) ?
						this.CurrentMediaFiles.Value.Except(x) :
						this.CurrentMediaFiles.Value.Union(x.Where(m => !this.CurrentMediaFiles.Value.Contains(m))).ToList();
			});

			// カレントアイテム→プロパティ,マップカレントアイテム片方向同期
			this.CurrentMediaFiles.Select(x => x.ToArray()).Subscribe(x => {
				this.Map.Value.CurrentMediaFiles.Value = x;
				this.MediaFileInformations.Value.Files.Value = x;
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
				.Throttle(TimeSpan.FromMilliseconds(100))
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
								this.LoadFileInDirectory(md, fsw.TokenSource.Token);
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
		/// <param name="cancellationToken">キャンセルトークン</param>
		protected abstract void LoadFileInDirectory(string directoryPath, CancellationToken cancellationToken);

		/// <summary>
		/// ファイルシステムイベント
		/// </summary>
		/// <param name="e">作成・更新・改名・削除などのイベント情報</param>
		protected abstract void OnFileSystemEvent(FileSystemEventArgs e);

		/// <summary>
		/// 表示モードの変更を行う。
		/// </summary>
		/// <param name="displayMode">変更後表示モード</param>
		public void ChangeDisplayMode(DisplayMode displayMode) {
			this.Settings.GeneralSettings.DisplayMode.Value = displayMode;
		}

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
			this._cancellationTokenSource.Cancel();
			base.Dispose(disposing);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
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
