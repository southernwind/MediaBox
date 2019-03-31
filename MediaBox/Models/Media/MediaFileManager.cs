using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Livet;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.EventAsObservable;
using SandBeige.MediaBox.Library.IO;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

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
		/// メディアファイル登録通知用Subject
		/// </summary>
		private readonly Subject<IEnumerable<IMediaFileModel>> _onRegisteredMediaFilesSubject = new Subject<IEnumerable<IMediaFileModel>>();

		/// <summary>
		/// タスク処理キュー
		/// </summary>
		private readonly PriorityTaskQueue _priorityTaskQueue = Get.Instance<PriorityTaskQueue>();

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
		/// メディアファイル登録通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnRegisteredMediaFiles {
			get {
				return this._onRegisteredMediaFilesSubject.AsObservable();
			}
		}

		private readonly ObservableSynchronizedCollection<(IMediaFileModel, MediaFile)> _waitingItems = new ObservableSynchronizedCollection<(IMediaFileModel, MediaFile)>();

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

			this._onFileSystemEventSubject.Subscribe(x => {
				if (!x.FullPath.IsTargetExtension()) {
					return;
				}

				switch (x.ChangeType) {
					case WatcherChangeTypes.Created:
						this.RegisterItem(this.MediaFactory.Create(x.FullPath));
						break;
				}
			});

			this._waitingItems
				.ToCollectionChanged<(IMediaFileModel model, MediaFile record)>()
				.Select(x => x.Value)
				.Synchronize()
				.Buffer(TimeSpan.FromSeconds(1), 1000)
				.Where(x => x.Any())
				.Subscribe(x => {
					lock (this.DataBase) {
						using (var transaction =
							this.DataBase.Database.BeginTransaction(IsolationLevel.ReadUncommitted)) {
							this.DataBase.MediaFiles.AddRange(x.Select(t => t.record));
							this.DataBase.SaveChanges();
							foreach (var (model, record) in x) {
								model.MediaFileId = record.MediaFileId;
							}
							transaction.Commit();
						}

						this._onRegisteredMediaFilesSubject.OnNext(x.Select(t => t.model));
					}
				});
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
						lock (this.DataBase) {
							var id = this.DataBase
								.MediaFiles
								.Include(x => x.AlbumMediaFiles)
								.FirstOrDefault(x => x.FilePath == mediaFile.FilePath)
								?.MediaFileId;

							if (id is { } notNullId) {
								mediaFile.MediaFileId = notNullId;
								return;
							}
						}

						// データ登録キューへ追加
						this._waitingItems.Add((mediaFile, mediaFile.CreateDataBaseRecord()));
					},
					Priority.LoadRegisteredAlbumOnRegister,
					this._cancellationTokenSource.Token
				)
			);
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
