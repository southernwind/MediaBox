using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Library.EventAsObservable;
using SandBeige.MediaBox.Library.IO;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

using static SandBeige.MediaBox.Models.Album.AlbumModel;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイル監視
	/// </summary>
	internal class MediaFileManager : ModelBase {
		/// <summary>
		/// ファイルシステムイベント用Subject
		/// </summary>
		private readonly Subject<FileSystemEventArgs> _onFileSystemEventSubject = new Subject<FileSystemEventArgs>();

		/// <summary>
		/// タスク処理キュー
		/// </summary>
		private readonly PriorityTaskQueue _priorityTaskQueue = new PriorityTaskQueue();

		/// <summary>
		/// キャンセルトークン Dispose時にキャンセルされる。
		/// </summary>
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		/// <summary>
		/// ファイルシステムイベント
		/// </summary>
		public IObservable<FileSystemEventArgs> OnFileSystemEvent {
			get {
				return this._onFileSystemEventSubject.AsObservable();
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileManager() {
			this.Settings
				.ScanSettings
				.ScanDirectories
				.ToReadOnlyReactiveCollection(sd => {
					if (!Directory.Exists(sd.DirectoryPath.Value)) {
						this.Logging.Log($"監視フォルダが見つかりません。{sd.DirectoryPath.Value}", LogLevel.Warning);
						return null;
					}
					var fsw = new Fsw {
						TokenSource = new CancellationTokenSource(),
						FileSystemWatcher = new FileSystemWatcher(sd.DirectoryPath.Value) {
							IncludeSubdirectories = sd.IncludeSubdirectories.Value,
							EnableRaisingEvents = sd.EnableMonitoring.Value
						}
					};

					// TODO:fsw.Taskが完了するまではイベントを溜め込んでおけるような仕組みにする
					var disposable = Observable.Merge(
						fsw.FileSystemWatcher.CreatedAsObservable(),
						fsw.FileSystemWatcher.RenamedAsObservable(),
						fsw.FileSystemWatcher.ChangedAsObservable(),
						fsw.FileSystemWatcher.DeletedAsObservable()
						).Subscribe(this._onFileSystemEventSubject.OnNext);

					fsw.FileSystemWatcher.DisposedAsObservable().Subscribe(_ => disposable.Dispose());
					fsw.Task = Task.Run(async () => {
						await Observable
							.Start(() => {
								this.LoadFileInDirectory(sd.DirectoryPath.Value, fsw.TokenSource.Token);
							})
							.FirstAsync();
					});

					// TODO : Dispose
					sd.IncludeSubdirectories.Subscribe(x => fsw.FileSystemWatcher.IncludeSubdirectories = x);
					sd.EnableMonitoring.Subscribe(x => fsw.FileSystemWatcher.EnableRaisingEvents = x);

					return fsw;
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		/// <param name="cancellationToken">キャンセルトークン</param>
		private void LoadFileInDirectory(string directoryPath, CancellationToken cancellationToken) {
			var newItems = DirectoryEx
				.EnumerateFiles(directoryPath)
				.Where(x => x.IsTargetExtension())
				.Select(x => this.MediaFactory.Create(x));
			foreach (var item in newItems) {
				if (cancellationToken.IsCancellationRequested) {
					return;
				}
				this.RegisterItem(item);
			}
		}

		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="mediaFile">登録ファイル</param>
		private void RegisterItem(IMediaFileModel mediaFile) {
			this._priorityTaskQueue.AddTask(
				new TaskAction(
					$"データベース登録[{mediaFile.FileName}]",
					() => {
						// 情報取得
						mediaFile.GetFileInfoIfNotLoaded();
						mediaFile.CreateThumbnailIfNotExists();

						// データ登録
						lock (this.DataBase) {
							var mf =
								this.DataBase
									.MediaFiles
									.Include(x => x.AlbumMediaFiles)
									.SingleOrDefault(x => x.FilePath == mediaFile.FilePath) ??
								mediaFile.RegisterToDataBase();
							this.DataBase.SaveChanges();
						}
					},
					Priority.LoadRegisteredAlbumOnRegister,
					this._cancellationTokenSource.Token
				)
			);
		}
	}
}
