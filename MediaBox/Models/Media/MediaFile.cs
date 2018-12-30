using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Creator;
using SandBeige.MediaBox.Library.Exif;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルクラス
	/// </summary>
	internal class MediaFile : ModelBase {
		private CancellationTokenSource _loadImageCancelToken;

		/// <summary>
		/// メディアファイルID
		/// </summary>
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
		/// 表示用画像
		/// </summary>
		public ReactivePropertySlim<ImageSource> Image {
			get;
		} = new ReactivePropertySlim<ImageSource>();

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
		/// もしまだ存在していなければ、サムネイル作成
		/// </summary>
		/// <param name="thumbnailLocation">サムネイル作成場所</param>
		public void CreateThumbnailIfNotExists(ThumbnailLocation thumbnailLocation) {
			if (this.Thumbnail.Value == null) {
				this.CreateThumbnail(thumbnailLocation);
			}
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="thumbnailLocation">サムネイル作成場所</param>
		public void CreateThumbnail(ThumbnailLocation thumbnailLocation) {
			if (thumbnailLocation == ThumbnailLocation.File) {
				byte[] thumbnailByteArray;
				if (this.Thumbnail.Value?.Image != null) {
					// メモリ上に展開されている場合はそっちを使う
					thumbnailByteArray = this.Thumbnail.Value.Image;
				} else {
					// なければ作る
					using (var fs = File.OpenRead(this.FilePath.Value)) {
						thumbnailByteArray = ThumbnailCreator.Create(fs, this.Settings.GeneralSettings.ThumbnailWidth.Value, this.Settings.GeneralSettings.ThumbnailHeight.Value);
					}
				}
				using (var crypto = new SHA256CryptoServiceProvider()) {
					var thumbnail = Get.Instance<Thumbnail>($"{string.Join("", crypto.ComputeHash(thumbnailByteArray).Select(b => $"{b:X2}"))}.jpg");
					if (!File.Exists(thumbnail.FilePath)) {
						File.WriteAllBytes(thumbnail.FilePath, thumbnailByteArray);
					}
					this.Thumbnail.Value = thumbnail;
				}
			} else {
				using (var fs = File.OpenRead(this.FilePath.Value)) {
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
		}

		/// <summary>
		/// プロパティの内容をデータベースへ登録
		/// </summary>
		/// <returns>登録したレコード</returns>
		public DataBase.Tables.MediaFile RegisterToDataBase() {
			var mf = new DataBase.Tables.MediaFile {
				DirectoryPath = Path.GetDirectoryName(this.FilePath.Value),
				FileName = this.FileName.Value,
				ThumbnailFileName = this.Thumbnail.Value?.FileName,
				Latitude = this.Latitude.Value,
				Longitude = this.Longitude.Value,
				Orientation = this.Orientation.Value
			};
			this.DataBase.MediaFiles.Add(mf);
			this.DataBase.SaveChanges();
			this.MediaFileId = mf.MediaFileId;
			return mf;
		}

		/// <summary>
		/// データベースからプロパティ読み込み
		/// </summary>
		public void LoadFromDataBase() {
			var mf =
				this.DataBase
					.MediaFiles
					.Include(x => x.MediaFileTags)
					.ThenInclude(x => x.Tag)
					.SingleOrDefault(x => Path.Combine(x.DirectoryPath, x.FileName) == this.FilePath.Value);
			if (mf == null) {
				return;
			}
			this.LoadFromDataBase(mf);
		}

		/// <summary>
		/// データベースからプロパティ読み込み
		/// </summary>
		/// <param name="record">データベースレコード</param>
		public void LoadFromDataBase(DataBase.Tables.MediaFile record) {
			this.MediaFileId = record.MediaFileId;
			this.Thumbnail.Value = record.ThumbnailFileName != null ? Get.Instance<Thumbnail>(record.ThumbnailFileName) : null;
			this.Latitude.Value = record.Latitude;
			this.Longitude.Value = record.Longitude;
			this.Orientation.Value = record.Orientation;
			this.Tags.AddRange(record.MediaFileTags.Select(t => t.Tag.TagName));
		}

		/// <summary>
		/// もし読み込まれていなければ、Exif読み込み
		/// </summary>
		/// <returns>Task</returns>
		public void LoadExifIfNotLoaded() {
			if (this.Exif.Value == null) {
				this.LoadExif();
			}
		}

		/// <summary>
		/// Exif読み込み
		/// </summary>
		/// <returns>Task</returns>
		public void LoadExif() {
			var exif = new Exif(this.FilePath.Value);
			this.Exif.Value = exif;
			if (new object[] { exif.GPSLatitude, exif.GPSLongitude, exif.GPSLatitudeRef, exif.GPSLongitudeRef }.All(l => l != null)) {
				this.Latitude.Value = (exif.GPSLatitude[0] + exif.GPSLatitude[1] / 60 + exif.GPSLatitude[2] / 3600) * (exif.GPSLongitudeRef == "S" ? -1 : 1);
				this.Longitude.Value = (exif.GPSLongitude[0] + exif.GPSLongitude[1] / 60 + exif.GPSLongitude[2] / 3600) * (exif.GPSLongitudeRef == "W" ? -1 : 1);
			}

			this.Orientation.Value = exif.Orientation;
		}

		/// <summary>
		/// 画像読み込み
		/// </summary>
		public async Task LoadImageAsync() {
			if (this._loadImageCancelToken != null) {
				return;
			}
			this._loadImageCancelToken = new CancellationTokenSource();
			this.Image.Value =
				await ImageSourceCreator.CreateAsync(
					this.FilePath.Value,
					this.Orientation.Value,
					token: this._loadImageCancelToken.Token);
			this._loadImageCancelToken = null;

		}

		/// <summary>
		/// 読み込んだ画像破棄
		/// </summary>
		public void UnloadImage() {
			this._loadImageCancelToken?.Cancel();
			this.Image.Value = null;
		}

		/// <summary>
		/// GPS情報登録
		/// </summary>
		/// <param name="latitude">緯度</param>
		/// <param name="longitude">経度</param>
		public void SetGps(double? latitude, double? longitude) {
			if (!this.MediaFileId.HasValue) {
				return;
			}

			this.Latitude.Value = latitude;
			this.Longitude.Value = longitude;

			using (var tran = this.DataBase.Database.BeginTransaction()) {
				var mf = this.DataBase.MediaFiles.Single(x => x.MediaFileId == this.MediaFileId.Value);
				mf.Latitude = this.Latitude.Value;
				mf.Longitude = this.Longitude.Value;
				this.DataBase.SaveChanges();
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
