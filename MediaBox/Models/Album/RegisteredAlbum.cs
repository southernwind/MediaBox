using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.EntityFrameworkCore;
using Reactive.Bindings;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	class RegisteredAlbum : Album {
		private int _albumId;

		/// <summary>
		/// 初期処理
		/// </summary>
		/// <returns>this</returns>
		public RegisteredAlbum Initialize(int? albumId = null) {
			if (albumId == null) {
				this.CreateAlbum();
			} else {
				this._albumId = (int)albumId;
			}
			this.LoadRegisteredInformation();
			this.BeginMonitoring();
			this.Title.Subscribe(x => {
				var album = this.DataBase.Albums.Single(a => a.AlbumId == this._albumId);
				album.Title = x;
				this.DataBase.SaveChanges();
			});

			this.MonitoringDirectories
				.ToCollectionChanged()
				.Subscribe(x => {
					var album = this.DataBase.Albums.Include(a => a.AlbumDirectories).Single(a => a.AlbumId == this._albumId);
					if (x.Action == NotifyCollectionChangedAction.Add) {
						album.AlbumDirectories.Add(new DataBase.Tables.AlbumDirectory() {
							Directory = x.Value.DirectoryPath.Value
						});
					} else if (x.Action == NotifyCollectionChangedAction.Remove) {
						this.DataBase.Remove(album.AlbumDirectories.Single(a => a.Directory == x.Value.DirectoryPath.Value));
					}
					this.DataBase.SaveChanges();
				});
			return this;
		}

		/// <summary>
		/// 新規アルバム作成
		/// </summary>
		private void CreateAlbum() {
			var album = new DataBase.Tables.Album();
			this.DataBase.Add(album);
			this.DataBase.SaveChanges();
			this._albumId = album.AlbumId;
		}

		/// <summary>
		/// データベースから登録済み情報の読み込み
		/// </summary>
		private void LoadRegisteredInformation() {
			var album =
				this.DataBase
					.Albums
					.Include(x => x.AlbumDirectories)
					.Where(x => x.AlbumId == this._albumId)
					.Select(x => new { x.Title, x.AlbumDirectories })
					.Single();

			this.Title.Value = album.Title;
			this.MonitoringDirectories.AddRange(
				album.AlbumDirectories.Select(x => {
					var md = Get.Instance<IMonitoringDirectory>();
					md.DirectoryPath.Value = x.Directory;
					md.Monitoring.Value = true;
					return md;
				})
			);

			this.Items.AddRange(
				this.DataBase
					.MediaFiles
					.Where(mf => mf.AlbumMediaFiles.Any(amf => amf.AlbumId == this._albumId))
					.Include(mf => mf.MediaFileTags)
					.ThenInclude(mft => mft.Tag)
					.AsEnumerable()
					.Select(x => {
						var m = Get.Instance<MediaFile>().Initialize(ThumbnailLocation.File, Path.Combine(x.DirectoryPath, x.FileName));
						m.MediaFileId = x.MediaFileId;
						m.Thumbnail.Value = Get.Instance<Thumbnail>().Initialize(x.ThumbnailFileName);
						m.Latitude.Value = x.Latitude;
						m.Longitude.Value = x.Longitude;
						m.Tags.AddRange(x.MediaFileTags.Select(t => t.Tag.TagName));
						return m;
					})
			);
		}

		/// <summary>
		/// アルバムへファイル追加
		/// </summary>
		public void AddFile(MediaFile mediaFile) {
			this.AddItem(mediaFile);
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		protected override void Load(string directoryPath) {
			if (!Directory.Exists(directoryPath)) {
				return;
			}
			this.Queue.AddRange(
				Directory
					.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
					.Where(x => x.IsTargetExtension())
					.Where(x => this.Queue.Union(this.Items).All(m => m.FilePath.Value != x))
					.Select(x => Get.Instance<MediaFile>().Initialize(ThumbnailLocation.File, x))
					.ToList());
		}

		/// <summary>
		/// メディアファイル追加
		/// </summary>
		/// <param name="mediaFile"></param>
		protected override void AddItem(MediaFile mediaFile) {
			mediaFile.CreateThumbnail();
			mediaFile.LoadExif();
			this.Items.Add(mediaFile);
			var dbmf = new DataBase.Tables.MediaFile() {
				DirectoryPath = Path.GetDirectoryName(mediaFile.FilePath.Value),
				FileName = mediaFile.FileName.Value,
				ThumbnailFileName = mediaFile.Thumbnail.Value.FileName,
				Latitude = mediaFile.Latitude.Value,
				Longitude = mediaFile.Longitude.Value
			};
			this.DataBase.AlbumMediaFiles.Add(new DataBase.Tables.AlbumMediaFile() {
				AlbumId = this._albumId,
				MediaFile = dbmf
			});
			this.DataBase.SaveChanges();
			mediaFile.MediaFileId = dbmf.MediaFileId;
		}
	}
}
