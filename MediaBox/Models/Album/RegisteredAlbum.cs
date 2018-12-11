﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactive.Bindings;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using MediaFile = SandBeige.MediaBox.Models.Media.MediaFile;

namespace SandBeige.MediaBox.Models.Album {
	internal class RegisteredAlbum : Album {
		private static readonly object _lockObject = new object();
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
		/// <param name="albumId">アルバムID</param>
		public RegisteredAlbum() {
			// TODO : ロード時に更新されてしまうのでなんとかする
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
					if (x.Action == NotifyCollectionChangedAction.Add) {
						album.AlbumDirectories.Add(new DataBase.Tables.AlbumDirectory() {
							Directory = x.Value
						});
					} else if (x.Action == NotifyCollectionChangedAction.Remove) {
						this.DataBase.Remove(album.AlbumDirectories.Single(a => a.Directory == x.Value));
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
						var m = Get.Instance<MediaFile>(Path.Combine(x.DirectoryPath, x.FileName));
						m.MediaFileId = x.MediaFileId;
						m.Thumbnail.Value = Get.Instance<Thumbnail>(x.ThumbnailFileName);
						m.Latitude.Value = x.Latitude;
						m.Longitude.Value = x.Longitude;
						m.Tags.AddRange(x.MediaFileTags.Select(t => t.Tag.TagName));
						return m;
					})
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
					.Select(x => Get.Instance<MediaFile>(x))
					.ToList());
		}

		/// <summary>
		/// メディアファイル追加
		/// </summary>
		/// <param name="mediaFile"></param>
		protected override async Task OnAddedItemAsync(MediaFile mediaFile) {
			if (mediaFile.MediaFileId != default) {
				return;
			}
			await mediaFile.CreateThumbnailAsync(ThumbnailLocation.File);
			await mediaFile.LoadExifAsync();
			lock (_lockObject) {
					var dbmf = new DataBase.Tables.MediaFile() {
					DirectoryPath = Path.GetDirectoryName(mediaFile.FilePath.Value),
					FileName = mediaFile.FileName.Value,
					ThumbnailFileName = mediaFile.Thumbnail.Value.FileName,
					Latitude = mediaFile.Latitude.Value,
					Longitude = mediaFile.Longitude.Value,
					Orientation = mediaFile.Orientation.Value
				};
				this.DataBase.MediaFiles.Add(dbmf);

				this.DataBase.AlbumMediaFiles.Add(new DataBase.Tables.AlbumMediaFile() {
					AlbumId = this.AlbumId,
					MediaFile = dbmf
				});
				this.DataBase.SaveChanges();
				mediaFile.MediaFileId = dbmf.MediaFileId;
			}
		}
	}
}
