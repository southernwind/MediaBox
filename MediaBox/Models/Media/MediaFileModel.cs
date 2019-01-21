using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Exif;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルクラス
	/// </summary>
	internal abstract class MediaFileModel : ModelBase {
		private Thumbnail _thumbnail;
		private double? _latitude;
		private double? _longitude;
		private int? _orientation;
		private Exif _exif;
		private DateTime _date;
		private long? _fileSize;
		private int _rate;

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
		/// 拡張子
		/// </summary>
		public string Extension {
			get;
		}

		/// <summary>
		/// 対象外部ツール
		/// </summary>
		public ReadOnlyReactiveCollection<ExternalTool> ExternalTools {
			get {
				return Get.Instance<ExternalToolsFactory>().Create(this.Extension);
			}
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public string FilePath {
			get;
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
		/// 日付時刻
		/// </summary>
		public DateTime Date {
			get {
				return this._date;
			}
			set {
				if (this._date == value) {
					return;
				}
				this._date = value;
				this.RaisePropertyChanged();
			}
		}

		/// <summary>
		/// ファイルサイズ
		/// </summary>
		public long? FileSize {
			get {
				return this._fileSize;
			}
			set {
				if (this._fileSize == value) {
					return;
				}
				this._fileSize = value;
				this.RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 評価
		/// </summary>
		public int Rate {
			get {
				return this._rate;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._rate, value);
			}
		}

		/// <summary>
		/// 初期処理
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		public MediaFileModel(string filePath) {
			this.FilePath = filePath;
			this.FileName = Path.GetFileName(filePath);
			this.Extension = Path.GetExtension(filePath);
		}

		/// <summary>
		/// もしまだ存在していなければ、サムネイル作成
		/// </summary>
		/// <param name="thumbnailLocation">サムネイル作成場所</param>
		public virtual void CreateThumbnailIfNotExists(ThumbnailLocation thumbnailLocation) {
			if (this.Thumbnail == null || !this.Thumbnail.Location.HasFlag(thumbnailLocation)) {
				this.CreateThumbnail(thumbnailLocation);
			}
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="thumbnailLocation">サムネイル作成場所</param>
		public virtual void CreateThumbnail(ThumbnailLocation thumbnailLocation) {
			try {
				this.Thumbnail = Get.Instance<ThumbnailPool>().ResolveOrRegisterByFullSizeFilePath(this.FilePath, thumbnailLocation);
			} catch (ArgumentException) {
				// TODO : ログ出力だけでいいのか、検討
				this.Logging.Log($"{this.FilePath}画像が不正なため、サムネイルの作成に失敗しました。");
			}
		}

		/// <summary>
		/// プロパティの内容をデータベースへ登録
		/// </summary>
		/// <returns>登録したレコード</returns>
		public MediaFile RegisterToDataBase() {
			var mf = new MediaFile {
				FilePath = this.FilePath,
				ThumbnailFileName = this.Thumbnail?.FileName,
				Latitude = this.Latitude,
				Longitude = this.Longitude,
				Date = this.Date,
				Orientation = this.Orientation,
				FileSize = this.FileSize,
				Rate = this.Rate
			};
			lock (this.DataBase) {
				this.DataBase.MediaFiles.Add(mf);
				this.DataBase.SaveChanges();
			}
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
					.SingleOrDefault(x => x.FilePath == this.FilePath);
			if (mf == null) {
				return;
			}
			this.LoadFromDataBase(mf);
		}

		/// <summary>
		/// データベースからプロパティ読み込み
		/// </summary>
		/// <param name="record">データベースレコード</param>
		public virtual void LoadFromDataBase(MediaFile record) {
			this.MediaFileId = record.MediaFileId;
			this.Thumbnail = record.ThumbnailFileName != null ? Get.Instance<ThumbnailPool>().ResolveOrRegisterByThumbnailFileName(this.FilePath, record.ThumbnailFileName) : null;
			this.Latitude = record.Latitude;
			this.Longitude = record.Longitude;
			this.Orientation = record.Orientation;
			this.Date = record.Date;
			this.FileSize = record.FileSize;
			this.Rate = record.Rate;
			this.Tags.Clear();
			this.Tags.AddRange(record.MediaFileTags.Select(t => t.Tag.TagName));
		}

		public void UpdateRate() {
			var mf =
				this.DataBase
					.MediaFiles
					.SingleOrDefault(x => x.FilePath == this.FilePath);

			lock (this.DataBase) {
				mf.Rate = this.Rate;
				this.DataBase.SaveChanges();
			}
		}

		/// <summary>
		/// もし読み込まれていなければ、ファイル情報読み込み
		/// </summary>
		/// <returns>Task</returns>
		public void GetFileInfoIfNotLoaded() {
			if (this.Exif == null) {
				this.GetFileInfo();
			}
		}

		/// <summary>
		/// ファイル情報読み込み
		/// </summary>
		/// <returns>Task</returns>
		public virtual void GetFileInfo() {
			this.Logging.Log($"[Exif Load]{this.FileName}");
			var exif = new Exif(this.FilePath);
			this.Exif = exif;
			if (new object[] { exif.GPSLatitude, exif.GPSLongitude, exif.GPSLatitudeRef, exif.GPSLongitudeRef }.All(l => l != null)) {
				this.Latitude = (exif.GPSLatitude[0] + (exif.GPSLatitude[1] / 60) + (exif.GPSLatitude[2] / 3600)) * (exif.GPSLongitudeRef == "S" ? -1 : 1);
				this.Longitude = (exif.GPSLongitude[0] + (exif.GPSLongitude[1] / 60) + (exif.GPSLongitude[2] / 3600)) * (exif.GPSLongitudeRef == "W" ? -1 : 1);
			}
			var fileInfo = new FileInfo(this.FilePath);
			this.Date = exif.DateTime == null ? fileInfo.CreationTime : DateTime.ParseExact(exif.DateTime, new[]{
				"yyyy:MM:dd HH:mm:ss",
				"yyyy:MM:dd HH:mm:ss.fff"
			}, null, default);
			this.Orientation = exif.Orientation;
			this.FileSize = fileInfo.Length;
		}

		/// <summary>
		/// サムネイル再作成
		/// </summary>
		public virtual void RecreateThumbnail() {
			this.Thumbnail.FullSizeFilePath = this.FilePath;
			this.Thumbnail.RecreateThumbnail();
		}

		/// <summary>
		/// 画像読み込み
		/// </summary>
		public virtual Task LoadImageAsync() {
			return Task.FromResult(default(object));
		}

		/// <summary>
		/// 読み込んだ画像破棄
		/// </summary>
		public virtual void UnloadImage() {
		}
	}

	/// <summary>
	/// サムネイル生成場所
	/// </summary>
	[Flags]
	public enum ThumbnailLocation {
		/// <summary>
		/// なし
		/// </summary>
		None = 0x0,
		/// <summary>
		/// ファイル
		/// </summary>
		File = 0x1,
		/// <summary>
		/// メモリ上
		/// </summary>
		Memory = 0x2
	}
}
