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
		private ImageSource _image;
		private Thumbnail _thumbnail;
		private double? _latitude;
		private double? _longitude;
		private int? _orientation;
		private Exif _exif;

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
		public string FileName {
			get;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public string FilePath {
			get;
		}

		/// <summary>
		/// 表示用画像
		/// </summary>
		public ImageSource Image {
			get {
				return this._image;
			}
			set {
				if (this._image == value) {
					return;
				}
				this._image = value;
				this.RaisePropertyChanged();
			}
		}

		/// <summary>
		/// サムネイル
		/// </summary>
		public Thumbnail Thumbnail {
			get {
				return this._thumbnail;
			}
			set {
				if (this._thumbnail == value) {
					return;
				}
				this._thumbnail = value;

				// TODO : Orientationの扱いを考える
				if (this.Thumbnail != null) {
					this.Thumbnail.Orientation = this.Orientation;
				}
				this.RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 緯度
		/// </summary>
		public double? Latitude {
			get {
				return this._latitude;
			}
			set {
				if (this._latitude == value) {
					return;
				}
				this._latitude = value;
				this.RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 経度
		/// </summary>
		public double? Longitude {
			get {
				return this._longitude;
			}
			set {
				if (this._longitude == value) {
					return;
				}
				this._longitude = value;
				this.RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 画像の回転
		/// </summary>
		public int? Orientation {
			get {
				return this._orientation;
			}
			set {
				if (this._orientation == value) {
					return;
				}
				this._orientation = value;

				// TODO : Orientationの扱いを考える
				if (this.Thumbnail != null) {
					this.Thumbnail.Orientation = this.Orientation;
				}
				this.RaisePropertyChanged();
			}
		}

		/// <summary>
		/// Exif情報
		/// </summary>
		public Exif Exif {
			get {
				return this._exif;
			}
			private set {
				if (this._exif == value) {
					return;
				}
				this._exif = value;
				this.RaisePropertyChanged();
			}
		}

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
			this.FilePath = filePath;
			this.FileName = Path.GetFileName(filePath);

		}

		/// <summary>
		/// もしまだ存在していなければ、サムネイル作成
		/// </summary>
		/// <param name="thumbnailLocation">サムネイル作成場所</param>
		public void CreateThumbnailIfNotExists(ThumbnailLocation thumbnailLocation) {
			if (this.Thumbnail == null) {
				this.CreateThumbnail(thumbnailLocation);
			}
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="thumbnailLocation">サムネイル作成場所</param>
		public void CreateThumbnail(ThumbnailLocation thumbnailLocation) {
			try {

				if (thumbnailLocation == ThumbnailLocation.File) {
					byte[] thumbnailByteArray;
					if (this.Thumbnail?.Image != null) {
						// メモリ上に展開されている場合はそっちを使う
						thumbnailByteArray = this.Thumbnail.Image;
					} else {
						// なければ作る
						using (var fs = File.OpenRead(this.FilePath)) {
							thumbnailByteArray = ThumbnailCreator.Create(fs, this.Settings.GeneralSettings.ThumbnailWidth.Value, this.Settings.GeneralSettings.ThumbnailHeight.Value);
						}
					}
					using (var crypto = new SHA256CryptoServiceProvider()) {
						var thumbnail = Get.Instance<Thumbnail>($"{string.Join("", crypto.ComputeHash(thumbnailByteArray).Select(b => $"{b:X2}"))}.jpg");
						if (!File.Exists(thumbnail.FilePath)) {
							File.WriteAllBytes(thumbnail.FilePath, thumbnailByteArray);
						}
						this.Thumbnail = thumbnail;
					}
				} else {
					// インメモリの場合、サムネイルプールから画像を取得する。
					this.Thumbnail =
						Get.Instance<Thumbnail>(
							Get.Instance<ThumbnailPool>().ResolveOrRegister(
								this.FilePath,
								() => {
									using (var fs = File.OpenRead(this.FilePath)) {
										return ThumbnailCreator.Create(fs, this.Settings.GeneralSettings.ThumbnailWidth.Value, this.Settings.GeneralSettings.ThumbnailHeight.Value);
									}
								}
							)
						);
				}
			} catch (ArgumentException) {
				// TODO : ログ出力だけでいいのか、検討
				this.Logging.Log($"{this.FilePath}画像が不正なため、サムネイルの作成に失敗しました。");
			}
		}

		/// <summary>
		/// プロパティの内容をデータベースへ登録
		/// </summary>
		/// <returns>登録したレコード</returns>
		public DataBase.Tables.MediaFile RegisterToDataBase() {
			var mf = new DataBase.Tables.MediaFile {
				DirectoryPath = Path.GetDirectoryName(this.FilePath),
				FileName = this.FileName,
				ThumbnailFileName = this.Thumbnail?.FileName,
				Latitude = this.Latitude,
				Longitude = this.Longitude,
				Orientation = this.Orientation
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
					.SingleOrDefault(x => Path.Combine(x.DirectoryPath, x.FileName) == this.FilePath);
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
			this.Thumbnail = record.ThumbnailFileName != null ? Get.Instance<Thumbnail>(record.ThumbnailFileName) : null;
			this.Latitude = record.Latitude;
			this.Longitude = record.Longitude;
			this.Orientation = record.Orientation;
			this.Tags.Clear();
			this.Tags.AddRange(record.MediaFileTags.Select(t => t.Tag.TagName));
		}

		/// <summary>
		/// もし読み込まれていなければ、Exif読み込み
		/// </summary>
		/// <returns>Task</returns>
		public void LoadExifIfNotLoaded() {
			if (this.Exif == null) {
				this.LoadExif();
			}
		}

		/// <summary>
		/// Exif読み込み
		/// </summary>
		/// <returns>Task</returns>
		public void LoadExif() {
			this.Logging.Log($"[Exif Load]{this.FileName}");
			var exif = new Exif(this.FilePath);
			this.Exif = exif;
			if (new object[] { exif.GPSLatitude, exif.GPSLongitude, exif.GPSLatitudeRef, exif.GPSLongitudeRef }.All(l => l != null)) {
				this.Latitude = (exif.GPSLatitude[0] + (exif.GPSLatitude[1] / 60) + (exif.GPSLatitude[2] / 3600)) * (exif.GPSLongitudeRef == "S" ? -1 : 1);
				this.Longitude = (exif.GPSLongitude[0] + (exif.GPSLongitude[1] / 60) + (exif.GPSLongitude[2] / 3600)) * (exif.GPSLongitudeRef == "W" ? -1 : 1);
			}
			this.Orientation = exif.Orientation;
		}

		/// <summary>
		/// 画像読み込み
		/// </summary>
		public async Task LoadImageAsync() {
			if (this._loadImageCancelToken != null) {
				return;
			}
			this._loadImageCancelToken = new CancellationTokenSource();
			this.Image =
				await ImageSourceCreator.CreateAsync(
					this.FilePath,
					this.Orientation,
					token: this._loadImageCancelToken.Token);
			this._loadImageCancelToken = null;

		}

		/// <summary>
		/// 読み込んだ画像破棄
		/// </summary>
		public void UnloadImage() {
			this._loadImageCancelToken?.Cancel();
			this.Image = null;
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
