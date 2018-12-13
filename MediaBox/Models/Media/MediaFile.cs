using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルクラス
	/// </summary>
	internal class MediaFile : ModelBase {

		public long? MediaFileId {
			get;
			set;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public ReadOnlyReactiveProperty<string> FileName {
			get;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public ReactivePropertySlim<string> FilePath {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// サムネイル
		/// </summary>
		public ReactivePropertySlim<Thumbnail> Thumbnail {
			get;
		} = new ReactivePropertySlim<Thumbnail>();

		/// <summary>
		/// 緯度
		/// </summary>
		public ReactivePropertySlim<double?> Latitude {
			get;
		} = new ReactivePropertySlim<double?>();

		/// <summary>
		/// 経度
		/// </summary>
		public ReactivePropertySlim<double?> Longitude {
			get;
		} = new ReactivePropertySlim<double?>();

		/// <summary>
		/// 画像の回転
		/// </summary>
		public ReactivePropertySlim<int?> Orientation {
			get;
		} = new ReactivePropertySlim<int?>();

		/// <summary>
		/// Exif情報
		/// </summary>
		public ReactiveProperty<Exif> Exif {
			get;
		} = new ReactiveProperty<Exif>();

		/// <summary>
		/// タグリスト
		/// </summary>
		public ReactiveCollection<string> Tags {
			get;
		} = new ReactiveCollection<string>();

		/// <summary>
		/// 初期処理
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		public MediaFile(string filePath) {
			this.FilePath.Value = filePath;
			this.FileName = this.FilePath.Select(Path.GetFileName).ToReadOnlyReactiveProperty();

			// TODO: サムネイルの回転情報について、もう少し考えたほうが良いかも？
			// サムネイルにも回転情報を伝える
			this.Orientation.Subscribe(x => {
				if (this.Thumbnail.Value != null) {
					this.Thumbnail.Value.Orientation = x;
				}
			});

			this.Thumbnail.Where(x => x != null).Subscribe(x => {
				x.Orientation = this.Orientation.Value;
			});
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		public async Task CreateThumbnailAsync(ThumbnailLocation thumbnailLocation) {
			await Task.Run(() => {
				using (var fs = File.OpenRead(this.FilePath.Value)) {
					if (thumbnailLocation == ThumbnailLocation.File) {
						var thumbnailByteArray = ThumbnailCreator.Create(fs, this.Settings.GeneralSettings.ThumbnailWidth.Value, this.Settings.GeneralSettings.ThumbnailHeight.Value);
						using (var crypto = new SHA256CryptoServiceProvider()) {
							var thumbnail = Get.Instance<Thumbnail>($"{string.Join("", crypto.ComputeHash(thumbnailByteArray).Select(b => $"{b:X2}"))}.jpg");
							if (!File.Exists(thumbnail.FilePath)) {
								File.WriteAllBytes(thumbnail.FilePath, thumbnailByteArray);
							}

							this.Thumbnail.Value = thumbnail;
						}
					} else {
						// インメモリの場合、サムネイルプールから画像を取得する。
						this.Thumbnail.Value =
							Get.Instance<Thumbnail>(
								Get.Instance<ThumbnailPool>().ResolveOrRegister(
									this.FilePath.Value,
									() => ThumbnailCreator.Create(fs, this.Settings.GeneralSettings.ThumbnailWidth.Value, this.Settings.GeneralSettings.ThumbnailHeight.Value)
								)
							);
					}
				}
			});
		}

		/// <summary>
		/// もし読み込まれていなければ、Exif読み込み
		/// </summary>
		/// <returns>Task</returns>
		public async Task LoadExifIfNotLoadedAsync() {
			if (this.Exif.Value == null) {
				await this.LoadExifAsync();
			}
		}

		/// <summary>
		/// Exif読み込み
		/// </summary>
		/// <returns>Task</returns>
		public async Task LoadExifAsync() {
			await Task.Run(() => {
				var exif = new Exif(this.FilePath.Value);
				this.Exif.Value = exif;
				if (new object[] { exif.GPSLatitude, exif.GPSLongitude, exif.GPSLatitudeRef, exif.GPSLongitudeRef }.All(l => l != null)) {
					this.Latitude.Value = (exif.GPSLatitude[0] + (exif.GPSLatitude[1] / 60) + (exif.GPSLatitude[2] / 3600)) * (exif.GPSLongitudeRef == "S" ? -1 : 1);
					this.Longitude.Value = (exif.GPSLongitude[0] + (exif.GPSLongitude[1] / 60) + (exif.GPSLongitude[2] / 3600)) * (exif.GPSLongitudeRef == "W" ? -1 : 1);
					this.Orientation.Value = exif.Orientation;
				}
			});
		}

		/// <summary>
		/// タグ追加
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public void AddTag(string tagName) {
			if (this.Tags.Contains(tagName)) {
				return;
			}
			if (!this.MediaFileId.HasValue) {
				return;
			}
			this.Tags.Add(tagName);
			var db = Get.Instance<MediaBoxDbContext>();
			using (var tran = db.Database.BeginTransaction()) {
				var mf = db.MediaFiles.Include(f => f.MediaFileTags).Single(x => x.MediaFileId == this.MediaFileId.Value);
				mf.MediaFileTags.Add(new MediaFileTag() {
					Tag = new Tag() {
						TagName = tagName
					}
				});
				db.SaveChanges();
				tran.Commit();
			}
		}

		/// <summary>
		/// タグ削除
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public void RemoveTag(string tagName) {
			if (!this.Tags.Contains(tagName)) {
				return;
			}
			if (!this.MediaFileId.HasValue) {
				return;
			}
			this.Tags.Remove(tagName);
			var db = Get.Instance<MediaBoxDbContext>();
			using (var tran = db.Database.BeginTransaction()) {
				var mft = db.MediaFileTags.Single(x => x.MediaFileId == this.MediaFileId.Value && x.Tag.TagName == tagName);
				db.Remove(mft);
				db.SaveChanges();
				tran.Commit();
			}
		}
	}

	/// <summary>
	/// サムネイル生成場所
	/// </summary>
	public enum ThumbnailLocation {
		/// <summary>
		/// ファイル
		/// </summary>
		File,
		/// <summary>
		/// メモリ上
		/// </summary>
		Memory
	}
}
