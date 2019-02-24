using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

using Livet;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.IO;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// 登録アルバム
	/// </summary>
	/// <remarks>
	/// ユーザーが作成したデータベースに登録するアルバム。
	/// 作成後、データベースに登録されるまでは<see cref="AlbumId"/>が0で、登録後に自動採番された値が入る。
	/// 
	/// </remarks>
	internal class RegisteredAlbum : AlbumModel {
		/// <summary>
		/// 登録待機中アイテム
		/// </summary>
		private readonly ObservableSynchronizedCollection<IMediaFileModel> _waitingItems = new ObservableSynchronizedCollection<IMediaFileModel>();

		/// <summary>
		/// アルバムID
		/// (subscribe時初期値配信なし)
		/// </summary>
		public IReactiveProperty<int> AlbumId {
			get;
		} = new ReactiveProperty<int>(mode: ReactivePropertyMode.DistinctUntilChanged);

		/// <summary>
		/// アルバム格納パス
		/// </summary>
		/// <remarks>
		/// "/picture/sea/shark"のような感じ。
		/// これをもとに<see cref="AlbumBox"/>が作られる。
		/// </remarks>
		public IReactiveProperty<string> AlbumPath {
			get;
		} = new ReactiveProperty<string>("");

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RegisteredAlbum() {
		}

		/// <summary>
		/// 新規アルバム作成
		/// </summary>
		public void Create() {
			var album = new DataBase.Tables.Album();
			lock (this.DataBase) {
				this.DataBase.Add(album);
				this.DataBase.SaveChanges();
			}
			this.AlbumId.Value = album.AlbumId;
		}

		/// <summary>
		/// データベースから登録済み情報の読み込み
		/// </summary>
		public void LoadFromDataBase(int albumId) {
			this.AlbumId.Value = albumId;
			var album =
				this.DataBase
					.Albums
					.Include(x => x.AlbumDirectories)
					.Where(x => x.AlbumId == this.AlbumId.Value)
					.Select(x => new { x.Title, x.Path, Directories = x.AlbumDirectories.Select(d => d.Directory) })
					.Single();

			lock (this.Items.SyncRoot) {
				this.Items.AddRange(
					this.DataBase
						.MediaFiles
						.Where(mf => mf.AlbumMediaFiles.Any(amf => amf.AlbumId == this.AlbumId.Value))
						.Include(mf => mf.MediaFileTags)
						.ThenInclude(mft => mft.Tag)
						.Include(mf => mf.ImageFile)
						.Include(mf => mf.VideoFile)
						.AsEnumerable()
						.Select(x => {
							var m = this.MediaFactory.Create(x.FilePath);
							m.LoadFromDataBase(x);
							return m;
						}).ToList()
				);
			}
			this.Title.Value = album.Title;
			this.AlbumPath.Value = album.Path;
			this.MonitoringDirectories.AddRange(album.Directories);

			// 非同期で順次ファイル情報の読み込みを行う
			var count = this.Items.Count;
			var lockObj = new object();
			foreach (var item in this.Items) {
				var ta = new TaskAction(
					item.GetFileInfoIfNotLoaded,
					Priority.LoadRegisteredAlbumOnLoad,
					this.CancellationToken
				);
				ta.OnTaskCompleted.Subscribe(_ => {
					lock (lockObj) {
						if (--count == 0) {
							this.OnInitializedSubject.OnNext(Unit.Default);
						}
					}
				});
				this.PriorityTaskQueue.AddTask(ta);
			}
			this.PriorityTaskQueue.StartTask();
		}

		/// <summary>
		/// アルバムプロパティ項目の編集をデータベースに反映する
		/// </summary>
		public void ReflectToDataBase() {
			lock (this.DataBase) {
				var album = this.DataBase.Albums.Include(a => a.AlbumDirectories).Single(a => a.AlbumId == this.AlbumId.Value);
				album.Title = this.Title.Value;
				album.Path = this.AlbumPath.Value;
				album.AlbumDirectories.Clear();
				album.AlbumDirectories.AddRange(this.MonitoringDirectories.Select(x =>
					new AlbumDirectory {
						Directory = x
					}));
				this.DataBase.SaveChanges();
			}
		}

		/// <summary>
		/// アルバムへファイル追加
		/// </summary>
		public void AddFiles(IEnumerable<IMediaFileModel> mediaFiles) {
			if (mediaFiles == null) {
				throw new ArgumentNullException();
			}
			foreach (var mediaFile in mediaFiles) {
				this.RegisterItem(mediaFile);
			}
			this.PriorityTaskQueue.StartTask();
		}

		/// <summary>
		/// アルバムからファイル削除
		/// </summary>
		/// <param name="mediaFiles"></param>
		public void RemoveFiles(IEnumerable<IMediaFileModel> mediaFiles) {
			if (mediaFiles == null) {
				throw new ArgumentNullException();
			}
			foreach (var file in mediaFiles.ToArray()) {
				this.RemoveFromDataBase(file);
				this.Items.Remove(file);
			}
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		/// <param name="cancellationToken">キャンセルトークン</param>
		protected override void LoadFileInDirectory(string directoryPath, CancellationToken cancellationToken) {
			var newItems = DirectoryEx
				.EnumerateFiles(directoryPath)
				.Where(x => x.IsTargetExtension())
				.Where(x => this.Items.Union(this._waitingItems).All(m => m.FilePath != x))
				.Select(x => this.MediaFactory.Create(x));
			foreach (var item in newItems) {
				if (cancellationToken.IsCancellationRequested) {
					return;
				}
				this.RegisterItem(item);
			}
			this.PriorityTaskQueue.StartTask();
		}

		/// <summary>
		/// ファイルシステムイベント
		/// </summary>
		/// <param name="e">作成・更新・改名・削除などのイベント情報</param>
		protected override void OnFileSystemEvent(FileSystemEventArgs e) {
			if (!e.FullPath.IsTargetExtension()) {
				return;
			}

			switch (e.ChangeType) {
				case WatcherChangeTypes.Created:
					this.RegisterItem(this.MediaFactory.Create(e.FullPath));
					this.PriorityTaskQueue.StartTask();
					break;
				case WatcherChangeTypes.Deleted:
					// TODO : 作成後すぐに削除されると、登録キューに入っていてItemsにはまだ入っていない可能性がある
					var target = this.Items.Single(i => i.FilePath == e.FullPath);
					this.RemoveFromDataBase(target);
					this.Items.Remove(target);
					break;
			}
		}

		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="mediaFile">登録ファイル</param>
		private void RegisterItem(IMediaFileModel mediaFile) {
			this._waitingItems.Add(mediaFile);
			this.PriorityTaskQueue.AddTask(
				new TaskAction(
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

							if (mf.AlbumMediaFiles?.All(x => x.AlbumId != this.AlbumId.Value) ?? true) {
								this.DataBase.AlbumMediaFiles.Add(new AlbumMediaFile {
									AlbumId = this.AlbumId.Value, // nullにはならない
									MediaFile = mf
								});
							}
							this.DataBase.SaveChanges();
						}

						// データ登録が終わったらこのアルバムのインスタンスに追加
						lock (this.Items.SyncRoot) {
							this.Items.Add(mediaFile);
						}
						// 登録完了したら待機中から削除
						this._waitingItems.Remove(mediaFile);
					},
					Priority.LoadRegisteredAlbumOnRegister,
					this.CancellationToken
				)
			);
		}

		/// <summary>
		/// DBから削除
		/// </summary>
		/// <param name="mediaFile">削除対象ファイル</param>
		private void RemoveFromDataBase(IMediaFileModel mediaFile) {
			lock (this.DataBase) {
				var mf = this.DataBase.AlbumMediaFiles.Single(x => x.AlbumId == this.AlbumId.Value && x.MediaFileId == mediaFile.MediaFileId);
				this.DataBase.AlbumMediaFiles.Remove(mf);
				this.DataBase.SaveChanges();
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}
	}
}
