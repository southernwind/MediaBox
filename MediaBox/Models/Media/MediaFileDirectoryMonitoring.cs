using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.EventAsObservable;
using SandBeige.MediaBox.Library.IO;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// フォルダ監視クラス
	/// </summary>
	internal class MediaFileDirectoryMonitoring : ModelBase {
		/// <summary>
		/// タスク処理キュー
		/// </summary>
		private readonly PriorityTaskQueue _priorityTaskQueue = Get.Instance<PriorityTaskQueue>();

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
		public IReadOnlyReactiveProperty<string> DirectoryPath {
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
		public MediaFileDirectoryMonitoring(ScanDirectory scanDirectory) {
			this.DirectoryPath = scanDirectory.DirectoryPath.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.IncludeSubdirectories = scanDirectory.IncludeSubdirectories.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.EnableMonitoring = scanDirectory.EnableMonitoring.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			var tokenSource = new CancellationTokenSource().AddTo(this.CompositeDisposable);
			var fileSystemWatcher = new FileSystemWatcher(this.DirectoryPath.Value) {
				IncludeSubdirectories = this.IncludeSubdirectories.Value,
				EnableRaisingEvents = this.EnableMonitoring.Value
			}.AddTo(this.CompositeDisposable);

			// TODO:fsw.Taskが完了するまではイベントを溜め込んでおけるような仕組みにする
			var disposable = Observable.Merge(
					fileSystemWatcher.CreatedAsObservable(),
					fileSystemWatcher.RenamedAsObservable(),
					fileSystemWatcher.ChangedAsObservable(),
					fileSystemWatcher.DeletedAsObservable()
				)
				.Subscribe(e => {
					if (!e.FullPath.IsTargetExtension()) {
						return;
					}

					// TODO : 登録前にイベントが発生する可能性があるので、Created以外は考慮する必要がある。
					switch (e.ChangeType) {
						case WatcherChangeTypes.Created:
							this._newFileNotificationSubject.OnNext(new[] { this.MediaFactory.Create(e.FullPath) });
							break;
						case WatcherChangeTypes.Changed:
							var mf = this.MediaFactory.Create(e.FullPath);
							if (mf.MediaFileId != null) {
								this._newFileNotificationSubject.OnNext(new[] { mf });
							}
							break;
						case WatcherChangeTypes.Deleted:
							this._deleteFileNotificationSubject.OnNext(new[] { this.MediaFactory.Create(e.FullPath) });
							break;
						case WatcherChangeTypes.Renamed:
							if (!(e is RenamedEventArgs rea)) {
								break;
							}
							this._deleteFileNotificationSubject.OnNext(new[] { this.MediaFactory.Create(rea.OldFullPath) });
							this._newFileNotificationSubject.OnNext(new[] { this.MediaFactory.Create(rea.FullPath) });
							break;
					}
				})
				.AddTo(this.CompositeDisposable);

			fileSystemWatcher.DisposedAsObservable().Subscribe(_ => disposable.Dispose());

			this.DirectoryPath
				.CombineLatest(this.IncludeSubdirectories, (directoryPath, includeSubdirectories) => (directoryPath, includeSubdirectories))
				.Subscribe(x => {
					this.LoadFileInDirectory(x.directoryPath, x.includeSubdirectories, tokenSource);
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
		/// <param name="cancellationTokenSource">キャンセルトークン</param>
		private void LoadFileInDirectory(string directoryPath, bool includeSubdirectories, CancellationTokenSource cancellationTokenSource) {
			var ta = new TaskAction($"データベース登録[{directoryPath}]",
				async state => await Task.Run(() => {
					(string path, long size)[] files;
					lock (this.DataBase) {
						files = this.DataBase
							.MediaFiles
							.Select(x => new { x.FilePath, x.FileSize })
							.AsEnumerable()
							.Select(x => (x.FilePath, x.FileSize))
							.ToArray();
					}

					var newItems = DirectoryEx
						.EnumerateFiles(directoryPath, includeSubdirectories)
						.Where(x => x.IsTargetExtension())
						.Where(x => !files.Any(f => x == f.path && new FileInfo(x).Length == f.size))
						.ToArray();

					state.ProgressMax.Value = newItems.Length;
					foreach (var item in newItems.Select(x => this.MediaFactory.Create(x)).Buffer(100)) {
						if (state.CancellationToken.IsCancellationRequested) {
							return;
						}
						this._newFileNotificationSubject.OnNext(item.ToArray());
						state.ProgressValue.Value += item.Count;
					}
				}),
				Priority.RegisterMediaFiles,
				cancellationTokenSource);
			this._priorityTaskQueue.AddTask(ta);
		}
	}
}
