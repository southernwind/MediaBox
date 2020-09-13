using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.Notification;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.IO;
using SandBeige.MediaBox.Models.Notification;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイル監視
	/// </summary>
	public class MediaFileManager : ModelBase, IMediaFileManager {
		private readonly ISettings _settings;
		private readonly IMediaFactory _mediaFactory;
		private readonly IMediaBoxDbContext _rdb;
		private readonly IDocumentDb _documentDb;
		private readonly INotificationManager _notificationManager;
		private readonly IPriorityTaskQueue _priorityTaskQueue;
		private readonly object _registerItemsLockObject = new object();
		/// <summary>
		/// メディアファイル登録通知用Subject
		/// </summary>
		private readonly Subject<IEnumerable<IMediaFileModel>> _onRegisteredMediaFilesSubject = new Subject<IEnumerable<IMediaFileModel>>();

		/// <summary>
		/// メディアファイル削除通知用Subject
		/// </summary>
		private readonly Subject<IEnumerable<IMediaFileModel>> _onDeletedMediaFilesSubject = new Subject<IEnumerable<IMediaFileModel>>();

		/// <summary>
		/// メディアファイル登録通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnRegisteredMediaFiles {
			get {
				return this._onRegisteredMediaFilesSubject.AsObservable();
			}
		}

		/// <summary>
		/// メディアファイル登録通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnDeletedMediaFiles {
			get {
				return this._onDeletedMediaFilesSubject.AsObservable();
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileManager(ISettings settings, IMediaFactory mediaFactory, ILogging logging, IMediaBoxDbContext rdb, IDocumentDb documentDb, INotificationManager notificationManager, IPriorityTaskQueue priorityTaskQueue) {
			this._settings = settings;
			this._mediaFactory = mediaFactory;
			this._rdb = rdb;
			this._documentDb = documentDb;
			this._notificationManager = notificationManager;
			this._priorityTaskQueue = priorityTaskQueue;
			this._settings
				.ScanSettings
				.ScanDirectories
				.ToReadOnlyReactiveCollection(sd => {
					var dm = new MediaFileDirectoryMonitoring(sd, this._mediaFactory, logging, this._rdb, this._documentDb, this._priorityTaskQueue, this._settings);
					dm.NewFileNotification.Subscribe(this.RegisterItemsCore);
					dm.DeleteFileNotification.Subscribe(x => {
						foreach (var item in x) {
							item.Exists = false;
						}
					});
					return dm;
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// データベースからファイルを削除
		/// </summary>
		/// <param name="mediaFiles">削除するファイル</param>
		public void DeleteItems(IEnumerable<IMediaFileModel> mediaFiles) {
			if (mediaFiles == null) {
				throw new ArgumentException();
			}
			if (mediaFiles.IsEmpty()) {
				return;
			}
			lock (this._registerItemsLockObject) {
				var files = mediaFiles.Select(x => x.FilePath);
				lock (this._rdb) {
					this._documentDb.GetMediaFilesCollection().DeleteMany(x => files.Contains(x.FilePath));
				}
				this._onDeletedMediaFilesSubject.OnNext(mediaFiles);
			}
		}


		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="directoryPath">登録するファイルを含んでいるフォルダパス</param>
		public void RegisterItems(string directoryPath) {
			if (directoryPath == null) {
				throw new ArgumentException();
			}
			if (!Directory.Exists(directoryPath)) {
				this._notificationManager.Notify(new Error(null, "存在しないディレクトリを読み込もうとしました。"));
			}
			this._priorityTaskQueue.AddTask(
				new TaskAction($"データベース登録[{directoryPath}]",
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
						.EnumerateFiles(directoryPath, true)
						.Where(x => x.IsTargetExtension(this._settings))
						.Where(x => !files.Any(f => x == f.path && new FileInfo(x).Length == f.size))
						.ToArray();

					state.ProgressMax.Value = newItems.Length;
					foreach (var item in newItems.Select(x => this._mediaFactory.Create(x)).Buffer(100)) {
						if (state.CancellationToken.IsCancellationRequested) {
							return;
						}
						this.RegisterItemsCore(item);
						state.ProgressValue.Value += item.Count;
					}
				}),
				Priority.RegisterMediaFiles,
				new CancellationTokenSource()));
		}

		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="mediaFilePaths">登録ファイル</param>
		public void RegisterItems(IEnumerable<string> mediaFilePaths) {
			if (mediaFilePaths == null) {
				throw new ArgumentException();
			}
			if (mediaFilePaths.IsEmpty()) {
				return;
			}
			this._priorityTaskQueue.AddTask(
				new TaskAction("データベース登録",
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

						var newMediaFiles = mediaFilePaths.Select(this._mediaFactory.Create)
							.Where(x => x.FilePath.IsTargetExtension(this._settings))
							.Where(x => !files.Any(f => x.FilePath == f.path && new FileInfo(x.FilePath).Length == f.size))
							.ToArray();

						state.ProgressMax.Value = newMediaFiles.Length;
						foreach (var item in newMediaFiles.Buffer(100)) {
							if (state.CancellationToken.IsCancellationRequested) {
								return;
							}
							this.RegisterItemsCore(item);
							state.ProgressValue.Value += item.Count;
						}
					}),
					Priority.RegisterMediaFiles,
					new CancellationTokenSource()));
		}

		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="mediaFiles">登録ファイル</param>
		private void RegisterItemsCore(IEnumerable<IMediaFileModel> mediaFiles) {
			lock (this._registerItemsLockObject) {
				var files = mediaFiles.Select(x => x.FilePath);
				var mediaFilesCollection = this._documentDb.GetMediaFilesCollection();
				MediaFile[] mfs;
				lock (this._rdb) {
					mfs = mediaFilesCollection
						.Query()
						.Include(x => x.Position)
						.Where(x => files.Contains(x.FilePath))
						.ToArray();
				}

				// データ登録キューへ追加
				var addList = new List<(IMediaFileModel model, MediaFile record)>();
				var updateList = new List<(IMediaFileModel model, MediaFile record)>();
				var joined =
					mediaFiles
						.GroupJoin(
							mfs,
							model => model.FilePath,
							record => record.FilePath,
							(x, y) => (model: x, record: y.FirstOrDefault())
						);
				foreach (var mf in joined) {
					if (mf.record != null) {
						updateList.Add(mf);
					} else {
						addList.Add((mf.model, mf.model.CreateDataBaseRecord()));
						this._notificationManager.Notify(new Information(mf.model.ThumbnailFilePath, $"ファイルが登録されました。{Environment.NewLine} [{mf.model.FileName}]"));
					}
				}
				lock (this._rdb) {
					mediaFilesCollection.Insert(addList.Select(t => t.record));

					foreach (var (model, record) in updateList) {
						model.UpdateDataBaseRecord(record);
					}

					// 必要な座標情報の事前登録
					var positionsCollection = this._documentDb.GetPositionsCollection();
					var prs = updateList
						.Union(addList)
						.Select(x => x.record)
						.Where(x => x.Latitude != null && x.Longitude != null)
						.Select(x => (Latitude: x.Latitude!.Value, Longitude: x.Longitude!.Value))
						.Except(positionsCollection.Query().ToList().Select(x => (x.Latitude, x.Longitude)))
						.Select(x => new Position() { Latitude = x.Latitude, Longitude = x.Longitude })
						.ToList();
					positionsCollection.Insert(prs);

					foreach (var (model, record) in addList) {
						model.MediaFileId = record.MediaFileId;
					}
				}

				this._onRegisteredMediaFilesSubject.OnNext(addList.Select(t => t.model));
			}
		}
	}
}
