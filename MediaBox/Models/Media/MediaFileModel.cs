using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルクラス
	/// </summary>
	/// <remarks>
	/// メディア間共通プロパティの定義と取得、登録を行う。
	/// </remarks>
	public abstract class MediaFileModel : ModelBase, IMediaFileModel {
		private ComparableSize? _resolution;
		private GpsLocation? _location;
		private DateTime _creationTime;
		private DateTime _modifiedTime;
		private DateTime _lastAccessTime;
		private string? _relativeThumbnailFilePath;
		private long _fileSize;
		private int _rate;
		private bool _isInvalid;
		private bool _exists = true;
		private readonly ISettings _settings;

		/// <summary>
		/// データベースから情報を取得済みか
		/// </summary>
		protected bool LoadedFromDataBase {
			get;
			private set;
		}

		/// <summary>
		/// 自動サムネイルか否か
		/// </summary>
		protected bool IsAutoGeneratedThumbnail {
			get;
			set;
		} = true;

		/// <summary>
		/// サムネイルが読み込み済みか
		/// </summary>
		public bool ThumbnailCreated {
			get;
			set;
		}

		/// <summary>
		/// ファイル情報取得済みか
		/// </summary>
		public bool FileInfoLoaded {
			get;
			set;
		}

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
		/// 拡張子
		/// </summary>
		public string Extension {
			get;
		}

		/// <summary>
		/// 作成日時
		/// </summary>
		public DateTime CreationTime {
			get {
				return this._creationTime;
			}
			set {
				this.SetProperty(ref this._creationTime, value);
			}
		}

		/// <summary>
		/// 編集日時
		/// </summary>
		public DateTime ModifiedTime {
			get {
				return this._modifiedTime;
			}
			set {
				this.SetProperty(ref this._modifiedTime, value);
			}
		}

		/// <summary>
		/// 最終アクセス日時
		/// </summary>
		public DateTime LastAccessTime {
			get {
				return this._lastAccessTime;
			}
			set {
				this.SetProperty(ref this._lastAccessTime, value);
			}
		}


		/// <summary>
		/// ファイルサイズ
		/// </summary>
		public long FileSize {
			get {
				return this._fileSize;
			}
			set {
				this.SetProperty(ref this._fileSize, value);
			}
		}

		/// <summary>
		/// サムネイル相対パス
		/// </summary>
		protected string? RelativeThumbnailFilePath {
			get {
				return this._relativeThumbnailFilePath;
			}
			set {
				this._relativeThumbnailFilePath = value;
				this.RaisePropertyChanged(nameof(this.ThumbnailFilePath));
			}
		}
		/// <summary>
		/// サムネイル
		/// </summary>
		public string? ThumbnailFilePath {
			get {
				if (this._relativeThumbnailFilePath == null) {
					return null;
				}
				return Path.Combine(this._settings.PathSettings.ThumbnailDirectoryPath.Value, this._relativeThumbnailFilePath);
			}
		}

		/// <summary>
		/// 解像度
		/// </summary>
		public ComparableSize? Resolution {
			get {
				return this._resolution;
			}
			set {
				this.SetProperty(ref this._resolution, value, nameof(this.Properties));
			}
		}

		/// <summary>
		/// 座標
		/// </summary>
		public GpsLocation? Location {
			get {
				return this._location;
			}
			set {
				this.SetProperty(ref this._location, value, nameof(this.Properties));
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
				this.SetProperty(ref this._rate, value, nameof(this.Properties));
			}
		}

		/// <summary>
		/// 不正なファイル
		/// </summary>
		public bool IsInvalid {
			get {
				return this._isInvalid;
			}
			set {
				this.SetProperty(ref this._isInvalid, value);
			}
		}

		/// <summary>
		/// タグリスト
		/// </summary>
		public ReactiveCollection<string> Tags {
			get;
		} = new ReactiveCollection<string>();

		/// <summary>
		/// プロパティ
		/// </summary>
		public virtual Attributes<string> Properties {
			get {
				return new Dictionary<string, string> {
					{ "作成日時",$"{this.CreationTime}" },
					{ "編集日時",$"{this.ModifiedTime}" },
					{ "最終アクセス日時",$"{this.LastAccessTime}" },
					{ "ファイルサイズ",$"{this.FileSize}" },
					{ "解像度" , $"{this.Resolution?.ToString()}" }
				}.ToAttributes();
			}
		}

		/// <summary>
		/// 存在するファイルか否か
		/// </summary>
		public bool Exists {
			get {
				return this._exists;
			}
			set {
				this.SetProperty(ref this._exists, value);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		protected MediaFileModel(string filePath, ISettings settings) {
			this._settings = settings;
			this.FilePath = filePath;
			this.FileName = Path.GetFileName(filePath);
			this.Extension = Path.GetExtension(filePath);
		}

		/// <summary>
		/// もしまだ存在していなければ、サムネイル作成
		/// </summary>
		public void CreateThumbnailIfNotExists() {
			if (this.ThumbnailCreated) {
				return;
			}
			if (!this.Exists) {
				return;
			}
			this.CreateThumbnail();
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		public virtual void CreateThumbnail() {
			this.ThumbnailCreated = true;
		}

		/// <summary>
		/// データベースからプロパティ読み込み
		/// </summary>
		/// <param name="record">データベースレコード</param>
		public virtual void LoadFromDataBase(MediaFile record) {
			this.MediaFileId = record.MediaFileId;
			this._relativeThumbnailFilePath = record.ThumbnailFileName;
			if (record.Latitude is { } lat && record.Longitude is { } lon) {
				this.Location = new GpsLocation(lat, lon, record.Altitude);
			} else {
				this.Location = null;
			}
			this.FileSize = record.FileSize;
			this.Rate = record.Rate;
			this.Resolution = new ComparableSize(record.Width, record.Height);
			this.IsInvalid = record.IsInvalid;
			this.IsAutoGeneratedThumbnail = record.IsAutoGeneratedThumbnail;
			this.Tags.Clear();
			this.Tags.AddRange(record.MediaFileTags.Select(t => t.Tag.TagName));
			this.RaisePropertyChanged(nameof(this.Tags));
			this.LoadedFromDataBase = true;
		}

		/// <summary>
		/// プロパティの内容からデータベースレコードを作成
		/// </summary>
		/// <returns>作成したレコード</returns>
		public MediaFile CreateDataBaseRecord() {
			var mf = new MediaFile();
			this.UpdateDataBaseRecord(mf);
			return mf;
		}

		/// <summary>
		/// プロパティの内容でデータベースレコードを更新
		/// </summary>
		/// <param name="targetRecord">更新対象レコード</param>
		public virtual void UpdateDataBaseRecord(MediaFile targetRecord) {
			this.UpdateFileInfo();
			if (this.IsAutoGeneratedThumbnail) {
				this.CreateThumbnail();
			}

			targetRecord.FilePath = this.FilePath;
			targetRecord.ThumbnailFileName = this._relativeThumbnailFilePath;
			targetRecord.Latitude = this.Location?.Latitude;
			targetRecord.Longitude = this.Location?.Longitude;
			targetRecord.Altitude = this.Location?.Altitude;
			targetRecord.DirectoryPath = $@"{Path.GetDirectoryName(this.FilePath)}\";
			targetRecord.FileSize = this.FileSize;
			targetRecord.Rate = this.Rate;
			targetRecord.Width = (int)(this.Resolution?.Width ?? 0);
			targetRecord.Height = (int)(this.Resolution?.Height ?? 0);
			targetRecord.IsInvalid = this.IsInvalid;
			targetRecord.IsAutoGeneratedThumbnail = this.IsAutoGeneratedThumbnail;
		}

		/// <summary>
		/// ファイル情報の取得
		/// </summary>
		public void UpdateFileInfo() {
			var fileInfo = new FileInfo(this.FilePath);
			this.Exists = fileInfo.Exists;
			if (this.Exists) {
				this.CreationTime = fileInfo.CreationTime;
				this.ModifiedTime = fileInfo.LastWriteTime;
				this.LastAccessTime = fileInfo.LastAccessTime;
				this.FileSize = fileInfo.Length;
			}
		}

		/// <summary>
		/// タグ追加
		/// </summary>
		/// <param name="tag">追加するタグ</param>
		public void AddTag(string tag) {
			this.Tags.Add(tag);
			this.RaisePropertyChanged(nameof(this.Tags));
		}

		/// <summary>
		/// タグ削除
		/// </summary>
		/// <param name="tag">削除するタグ</param>
		public void RemoveTag(string tag) {
			this.Tags.Remove(tag);
			this.RaisePropertyChanged(nameof(this.Tags));
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.FilePath}>";
		}
	}
}
