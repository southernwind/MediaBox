using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	internal class RegisteredAlbum : Album {
		private bool _isReady;

		/// <summary>
		/// アルバムID
		/// </summary>
		public int AlbumId {
			get;
			private set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RegisteredAlbum() {
			this.Title.Subscribe(x => {
				if (!this._isReady) {
					return;
				}
				var album = this.DataBase.Albums.Single(a => a.AlbumId == this.AlbumId);
				album.Title = x;
				this.DataBase.SaveChanges();
			});

			this.MonitoringDirectories
				.ToCollectionChanged()
				.Subscribe(x => {
					if (!this._isReady) {
						return;
					}
					var album = this.DataBase.Albums.Include(a => a.AlbumDirectories).Single(a => a.AlbumId == this.AlbumId);
					switch (x.Action) {
						case NotifyCollectionChangedAction.Add:
							album.AlbumDirectories.Add(new DataBase.Tables.AlbumDirectory {
								Directory = x.Value
							});
							break;
						case NotifyCollectionChangedAction.Remove:
							this.DataBase.Remove(album.AlbumDirectories.Single(a => a.Directory == x.Value));
							break;
					}
					this.DataBase.SaveChanges();
				});
		}

		/// <summary>
		/// 新規アルバム作成
		/// </summary>
		public void Create() {
			if (this._isReady) {
				throw new InvalidOperationException();
			}
			var album = new DataBase.Tables.Album();
			this.DataBase.Add(album);
			this.DataBase.SaveChanges();
			this.AlbumId = album.AlbumId;
			this._isReady = true;
		}

		/// <summary>
		/// データベースから登録済み情報の読み込み
		/// </summary>
		public void LoadFromDataBase(int albumId) {
			if (this._isReady) {
				throw new InvalidOperationException();
			}
			this.AlbumId = albumId;
			var album =
				this.DataBase
					.Albums
					.Include(x => x.AlbumDirectories)
					.Where(x => x.AlbumId == this.AlbumId)
					.Select(x => new { x.Title, Directories = x.AlbumDirectories.Select(d => d.Directory) })
					.Single();

			this.Items.AddRange(
				this.DataBase
					.MediaFiles
					.Where(mf => mf.AlbumMediaFiles.Any(amf => amf.AlbumId == this.AlbumId))
					.Include(mf => mf.MediaFileTags)
					.ThenInclude(mft => mft.Tag)
					.AsEnumerable()
					.Select(x => {
						var m = this.MediaFactory.Create(Path.Combine(x.DirectoryPath, x.FileName));
						m.LoadFromDataBase(x);
						return m;
					}).ToList()
			);

			this.Title.Value = album.Title;
			this.MonitoringDirectories.AddRange(album.Directories);
			this._isReady = true;
		}

		/// <summary>
		/// アルバムへファイル追加
		/// </summary>
		public void AddFiles(IEnumerable<MediaFile> mediaFiles) {
			if (!this._isReady) {
				throw new InvalidOperationException();
			}
			if (mediaFiles == null) {
				throw new ArgumentNullException();
			}
			this.Items.AddRangeOnScheduler(mediaFiles);
		}

		/// <summary>
		/// アルバムからファイル削除
		/// </summary>
		/// <param name="mediaFiles"></param>
		public void RemoveFiles(IEnumerable<MediaFile> mediaFiles) {
			if (!this._isReady) {
				throw new InvalidOperationException();
			}
			if (mediaFiles == null) {
				throw new ArgumentNullException();
			}
			foreach (var file in mediaFiles) {
				this.Items.Remove(file);
			}
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		protected override void LoadFileInDirectory(string directoryPath) {
			if (!Directory.Exists(directoryPath)) {
				return;
			}
			this.Items.AddRangeOnScheduler(
				Directory
					.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
					.Where(x => x.IsTargetExtension())
					.Where(x => this.Items.All(m => m.FilePath.Value != x))
					.Select(x => this.MediaFactory.Create(x))
					.ToList());
		}

		/// <summary>
		/// メディアファイル追加
		/// </summary>
		/// <param name="mediaFile"></param>
		protected override async Task OnAddedItemAsync(MediaFile mediaFile) {
			await mediaFile.CreateThumbnailAsync(ThumbnailLocation.File);
			await mediaFile.LoadExifAsync();
			lock (this.DataBase) {
				var mf =
					this.DataBase
						.MediaFiles
						.Include(x => x.AlbumMediaFiles)
						.SingleOrDefault(x => Path.Combine(x.DirectoryPath, x.FileName) == mediaFile.FilePath.Value);

				if (mf == null) {
					mf = mediaFile.RegisterToDataBase();
				}
				if (mf.AlbumMediaFiles?.All(x => x.AlbumId != this.AlbumId) ?? true) {
					this.DataBase.AlbumMediaFiles.Add(new DataBase.Tables.AlbumMediaFile {
						AlbumId = this.AlbumId,
						MediaFile = mf
					});
				}
				this.DataBase.SaveChanges();
			}
		}

		protected override Task OnRemovedItemAsync(MediaFile mediaFile) {
			lock (this.DataBase) {
				var mf = this.DataBase.AlbumMediaFiles.Single(x => x.MediaFileId == mediaFile.MediaFileId);
				this.DataBase.AlbumMediaFiles.Remove(mf);
				this.DataBase.SaveChanges();
			}
			return Task.FromResult(default(object));
		}
	}
}
