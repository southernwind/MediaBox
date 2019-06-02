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
		/// メディアファイル登録通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnRegisteredMediaFiles {
			get {
				return this._onRegisteredMediaFilesSubject.AsObservable();
			}
		}

		private readonly ObservableSynchronizedCollection<(Method, IMediaFileModel, MediaFile)> _waitingItems = new ObservableSynchronizedCollection<(Method, IMediaFileModel, MediaFile)>();

		/// <summary>
		/// 読み込み状態
		/// </summary>
		public ReadOnlyReactiveCollection<AsyncState> LoadStates {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileManager() {
			this.LoadStates = this.Settings
				.ScanSettings
				.ScanDirectories
				.ToReadOnlyReactiveCollection(sd => {
					if (!Directory.Exists(sd.DirectoryPath.Value)) {
						this.Logging.Log($"監視フォルダが見つかりません。{sd.DirectoryPath.Value}", LogLevel.Warning);
						return null;
					}
					var state = new AsyncState {
						TokenSource = new CancellationTokenSource(),
						FileSystemWatcher = new FileSystemWatcher(sd.DirectoryPath.Value) {
							IncludeSubdirectories = sd.IncludeSubdirectories.Value,
							EnableRaisingEvents = sd.EnableMonitoring.Value
						}
					};

					// TODO:fsw.Taskが完了するまではイベントを溜め込んでおけるような仕組みにする
					var disposable = Observable.Merge(
							state.FileSystemWatcher.CreatedAsObservable(),
							state.FileSystemWatcher.RenamedAsObservable(),
							state.FileSystemWatcher.ChangedAsObservable(),
							state.FileSystemWatcher.DeletedAsObservable()
						)
						.Subscribe(this._onFileSystemEventSubject.OnNext);

					state.FileSystemWatcher.DisposedAsObservable().Subscribe(_ => disposable.Dispose());
					state.Task = Task.Run(async () => {
						await Observable
							.Start(() => {
								this.LoadFileInDirectory(sd.DirectoryPath.Value, sd.IncludeSubdirectories.Value, state.TokenSource.Token);
							})
							.FirstAsync();
					});

					// TODO : Dispose
					sd.IncludeSubdirectories.Subscribe(x => state.FileSystemWatcher.IncludeSubdirectories = x);
					sd.EnableMonitoring.Subscribe(x => state.FileSystemWatcher.EnableRaisingEvents = x);

					return state;
				}).AddTo(this.CompositeDisposable);

			this._onFileSystemEventSubject.Subscribe(e => {
				if (!e.FullPath.IsTargetExtension()) {
					return;
				}

				// TODO : 登録前にイベントが発生する可能性があるので、Created以外は考慮する必要がある。
				switch (e.ChangeType) {
					case WatcherChangeTypes.Created:
						this.RegisterItems(new[] { this.MediaFactory.Create(e.FullPath) });
						break;
					case WatcherChangeTypes.Changed:
						var mf = this.MediaFactory.Create(e.FullPath);
						if (mf.MediaFileId != null) {
							this.RegisterItems(new[] { mf });
						}
						break;
					case WatcherChangeTypes.Deleted:
						this.MediaFactory.Create(e.FullPath).Exists = false;
						break;
					case WatcherChangeTypes.Renamed:
						if (!(e is RenamedEventArgs rea)) {
							break;
						}
						this.MediaFactory.Create(rea.OldFullPath).Exists = false;
						this.RegisterItems(new[] { this.MediaFactory.Create(rea.FullPath) });
						break;
				}
			}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		/// <param name="includeSubdirectories">サブディレクトリを含むか否か</param>
		/// <param name="cancellationToken">キャンセルトークン</param>
		private void LoadFileInDirectory(string directoryPath, bool includeSubdirectories, CancellationToken cancellationToken) {
			this._priorityTaskQueue.AddTask(
				new TaskAction("データベース登録",
				async () => await Task.Run(() => {
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
						.Select(x => this.MediaFactory.Create(x));
					foreach (var item in newItems.Buffer(100)) {
						if (cancellationToken.IsCancellationRequested) {
							return;
						}
						this.RegisterItems(item);
					}
				}),
				Priority.RegisterMediaFiles,
				this._cancellationTokenSource.Token)
			);
		}

		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="mediaFile">登録ファイル</param>
		private void RegisterItems(IEnumerable<IMediaFileModel> mediaFiles) {
			MediaFile[] mfs;
			var files = mediaFiles.Select(x => x.FilePath);
			lock (this.DataBase) {
				mfs = this.DataBase
					.MediaFiles
					.Include(x => x.AlbumMediaFiles)
					.Include(x => x.ImageFile)
					.Include(x => x.VideoFile)
					.Include(x => x.Jpeg)
					.Include(x => x.Png)
					.Include(x => x.Bmp)
					.Include(x => x.Gif)
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
				}
			}

			lock (this.DataBase) {
				using var transaction = this.DataBase.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
				this.DataBase.MediaFiles.AddRange(addList.Select(t => t.record));
				foreach (var (model, record) in updateList) {
					model.UpdateDataBaseRecord(record);
				}
				try {
					this.DataBase.SaveChanges();
				} catch (Exception ex) {
					throw;
				}
				foreach (var (model, record) in addList) {
					model.MediaFileId = record.MediaFileId;
				}
				transaction.Commit();
			}

			this._onRegisteredMediaFilesSubject.OnNext(addList.Select(t => t.model));
		}
	}

	internal enum Method {
		Register,
		Update
	}

	/// <summary>
	/// 非同期読み込み状態確認/破棄用オブジェクト
	/// </summary>
	/// <remarks>
	/// 確認は基本的にテストのため。テスト以外で使うならもうちょっとやり方を考える。
	/// </remarks>
	internal class AsyncState : IDisposable {
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
