using System;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Library.EventAsObservable;
using SandBeige.MediaBox.Library.IO;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// フォルダ監視クラス
	/// </summary>
	public class MediaFileDirectoryMonitoring : ModelBase {
		private readonly object _lockObj = new object();
		private readonly IMediaFactory _mediaFactory;
		private readonly ILogging _logging;
		private readonly IMediaBoxDbContext _rdb;
		private readonly IDocumentDb _documentDb;
		private readonly ISettings _settings;
		/// <summary>
		/// ファイル初期読み込みロード
		/// </summary>
		private TaskAction? _taskAction;

		/// <summary>
		/// タスク処理キュー
		/// </summary>
		private readonly IPriorityTaskQueue _priorityTaskQueue;

		private readonly Subject<IMediaFileModel[]> _newFileNotificationSubject = new Subject<IMediaFileModel[]>();
		/// <summary>
		/// 新規ファイル通知
		/// </summary>
		public IObservable<IMediaFileModel[]> NewFileNotification {
			get {
				return this._newFileNotificationSubject.AsObservable();
			}
		}

		private readonly Subject<IMediaFileModel[]> _deleteFileNotificationSubject = new Subject<IMediaFileModel[]>();

		/// <summary>
		/// ファイル削除通知
		/// </summary>
		public IObservable<IMediaFileModel[]> DeleteFileNotification {
			get {
				return this._deleteFileNotificationSubject.AsObservable();
			}
		}

		/// <summary>
		/// ディレクトリパス
		/// </summary>
		public string DirectoryPath {
			get;
		}

		/// <summary>
		/// サブディレクトリを含むか否か
		/// </summary>
		public IReadOnlyReactiveProperty<bool> IncludeSubdirectories {
			get;
		}

		/// <summary>
		/// 有効/無効
		/// </summary>
		public IReadOnlyReactiveProperty<bool> EnableMonitoring {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="scanDirectory">設定ファイルオブジェクト</param>
		public MediaFileDirectoryMonitoring(ScanDirectory scanDirectory, IMediaFactory mediaFactory, ILogging logging, IMediaBoxDbContext rdb, IDocumentDb documentDb, IPriorityTaskQueue priorityTaskQueue, ISettings settings) {
			this._mediaFactory = mediaFactory;
			this._logging = logging;
			this._rdb = rdb;
			this._documentDb = documentDb;
			this.DirectoryPath = scanDirectory.DirectoryPath.Value;
			this._priorityTaskQueue = priorityTaskQueue;
			this._settings = settings;
			if (!Directory.Exists(this.DirectoryPath)) {
				this._logging.Log($"監視フォルダが見つかりません。{this.DirectoryPath}", LogLevel.Warning);
				// TODO : エラーをどう伝えるか考える。例外でいいのか。
				throw new InvalidOperationException();
			}

			this.IncludeSubdirectories = scanDirectory.IncludeSubdirectories.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.EnableMonitoring = scanDirectory.EnableMonitoring.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			var fileSystemWatcher = new FileSystemWatcher().AddTo(this.CompositeDisposable);


			fileSystemWatcher.Path = this.DirectoryPath;
			fileSystemWatcher.IncludeSubdirectories = this.IncludeSubdirectories.Value;
			fileSystemWatcher.EnableRaisingEvents = this.EnableMonitoring.Value;

			// TODO:fsw.Taskが完了するまではイベントを溜め込んでおけるような仕組みにする
			var disposable = Observable.Merge(
					fileSystemWatcher.CreatedAsObservable(),
					fileSystemWatcher.RenamedAsObservable(),
					fileSystemWatcher.ChangedAsObservable(),
					fileSystemWatcher.DeletedAsObservable()
				)
				.Subscribe(e => {
					if (!e.FullPath.IsTargetExtension(this._settings)) {
						return;
					}

					// TODO : 登録前にイベントが発生する可能性があるので、Created以外は考慮する必要がある。
					switch (e.ChangeType) {
						case WatcherChangeTypes.Created:
							this._newFileNotificationSubject.OnNext(new[] { this._mediaFactory.Create(e.FullPath) });
							break;
						case WatcherChangeTypes.Changed:
							var mf = this._mediaFactory.Create(e.FullPath);
							if (mf.MediaFileId != null) {
								this._newFileNotificationSubject.OnNext(new[] { mf });
							}
							break;
						case WatcherChangeTypes.Deleted:
							this._deleteFileNotificationSubject.OnNext(new[] { this._mediaFactory.Create(e.FullPath) });
							break;
						case WatcherChangeTypes.Renamed:
							if (!(e is RenamedEventArgs rea)) {
								break;
							}
							this._deleteFileNotificationSubject.OnNext(new[] { this._mediaFactory.Create(rea.OldFullPath) });
							this._newFileNotificationSubject.OnNext(new[] { this._mediaFactory.Create(rea.FullPath) });
							break;
					}
				})
				.AddTo(this.CompositeDisposable);

			fileSystemWatcher.DisposedAsObservable().Subscribe(_ => disposable.Dispose());

			this.IncludeSubdirectories
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => {
					this.LoadFileInDirectory(this.DirectoryPath, x);
				}).AddTo(this.CompositeDisposable);

			this.IncludeSubdirectories
				.Subscribe(x => fileSystemWatcher.IncludeSubdirectories = x)
				.AddTo(this.CompositeDisposable);
			this.EnableMonitoring
				.Subscribe(x => fileSystemWatcher.EnableRaisingEvents = x)
				.AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		/// <param name="includeSubdirectories">サブディレクトリを含むか否か</param>
		private void LoadFileInDirectory(string directoryPath, bool includeSubdirectories) {
			lock (this._lockObj) {
				if (this._taskAction != null && this._taskAction.TaskState != TaskState.Done) {
					this._taskAction.CancellationTokenSource.Cancel();
					this._taskAction.OnTaskCompleted
						.ToUnit()
						.Merge(this._taskAction.OnError.ToUnit())
						.FirstAsync()
						.Wait();
				}

				this._taskAction = new TaskAction($"データベース登録[{directoryPath}]",
					async state => await Task.Run(() => {
						(string path, long size)[] files;
						lock (this._rdb) {
							files = this._documentDb
								.GetMediaFilesCollection()
								.Query()
								.Select(x => new { x.FilePath, x.FileSize })
								.ToEnumerable()
								.Select(x => (x.FilePath, x.FileSize))
								.ToArray();
						}

						var newItems = DirectoryEx
							.EnumerateFiles(directoryPath, includeSubdirectories)
							.Where(x => x.IsTargetExtension(this._settings))
							.Where(x => !files.Any(f => x == f.path && new FileInfo(x).Length == f.size))
							.ToArray();

						state.ProgressMax.Value = newItems.Length;
						foreach (var item in newItems.Select(x => this._mediaFactory.Create(x)).Buffer(100)) {
							if (state.CancellationToken.IsCancellationRequested) {
								return;
							}

							this._newFileNotificationSubject.OnNext(item.ToArray());
							state.ProgressValue.Value += item.Count;
						}
					}),
					Priority.RegisterMediaFiles,
					new CancellationTokenSource());
				this._priorityTaskQueue.AddTask(this._taskAction);
			}
		}

		protected override void Dispose(bool disposing) {
			this._taskAction?.Dispose();
			base.Dispose(disposing);
		}
	}
}
