using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	internal class RegisteredAlbum : AlbumModel {
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
		public IReactiveProperty<string> AlbumPath {
			get;
		} = new ReactiveProperty<string>("");

		/// <summary>
		/// データベース登録キュー
		/// subjectのOnNextで発火してitemsの中身をすべて登録する
		/// </summary>
		private (Subject<Unit> subject, IList<MediaFileModel> items) QueueOfRegisterToItems {
			get;
		} = (new Subject<Unit>(), new List<MediaFileModel>());

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RegisteredAlbum() {
			this.ThumbnailLocation = ThumbnailLocation.File;
			this.QueueOfRegisterToItems
				.subject
				.CombineLatest(this.AlbumId, (x, y) => x)
				.ObserveOnBackground(this.Settings.ForTestSettings.RunOnBackground.Value)
				.Subscribe(_ => {
					while (this.QueueOfRegisterToItems.items.Count != 0) {
						if (this.CancellationToken.IsCancellationRequested) {
							return;
						}
						var mediaFile = this.QueueOfRegisterToItems.items.FirstOrDefault();
						this.RegisterToDataBase(mediaFile);
						// 登録が終わったら追加
						this.Items.Add(mediaFile);
						this.QueueOfRegisterToItems.items.Remove(mediaFile);
					}
				}).AddTo(this.CompositeDisposable);
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
						var m = this.MediaFactory.Create(x.FilePath, this.ThumbnailLocation);
						m.LoadFromDataBase(x);
						return m;
					}).ToList()
			);

			this.Title.Value = album.Title;
			this.AlbumPath.Value = album.Path;
			this.MonitoringDirectories.AddRange(album.Directories);
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
		public void AddFiles(IEnumerable<MediaFileModel> mediaFiles) {
			if (mediaFiles == null) {
				throw new ArgumentNullException();
			}
			this.QueueOfRegisterToItems.items.AddRange(mediaFiles);
			this.QueueOfRegisterToItems.subject.OnNext(Unit.Default);
		}

		/// <summary>
		/// アルバムからファイル削除
		/// </summary>
		/// <param name="mediaFiles"></param>
		public void RemoveFiles(IEnumerable<MediaFileModel> mediaFiles) {
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
		protected override void LoadFileInDirectory(string directoryPath) {
			this.QueueOfRegisterToItems.items.AddRange(
				Directory
					.EnumerateFiles(directoryPath)
					.Where(x => x.IsTargetExtension())
					.Where(x => this.Items.ToArray().Union(this.QueueOfRegisterToItems.items.ToArray()).All(m => m.FilePath != x))
					.Select(x => this.MediaFactory.Create(x, this.ThumbnailLocation))
					.ToList());
			this.QueueOfRegisterToItems.subject.OnNext(Unit.Default);
		}

		protected override void OnFileSystemEvent(FileSystemEventArgs e) {
			if (!e.FullPath.IsTargetExtension()) {
				return;
			}

			switch (e.ChangeType) {
				case WatcherChangeTypes.Created:
					this.QueueOfRegisterToItems.items.Add(this.MediaFactory.Create(e.FullPath, this.ThumbnailLocation));
					this.QueueOfRegisterToItems.subject.OnNext(Unit.Default);
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
		/// DBに登録
		/// </summary>
		/// <param name="mediaFile">登録ファイル</param>
		/// <returns></returns>
		private void RegisterToDataBase(MediaFileModel mediaFile) {
			mediaFile.ThumbnailLocation |= this.ThumbnailLocation;
			mediaFile.GetFileInfoIfNotLoaded();
			mediaFile.CreateThumbnailIfNotExists();
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
		}

		/// <summary>
		/// DBから削除
		/// </summary>
		/// <param name="mediaFile">削除対象ファイル</param>
		/// <returns></returns>
		private void RemoveFromDataBase(MediaFileModel mediaFile) {
			lock (this.DataBase) {
				var mf = this.DataBase.AlbumMediaFiles.Single(x => x.AlbumId == this.AlbumId.Value && x.MediaFileId == mediaFile.MediaFileId);
				this.DataBase.AlbumMediaFiles.Remove(mf);
				this.DataBase.SaveChanges();
			}
		}
	}
}
