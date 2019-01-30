using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Collection;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルクラス
	/// </summary>
	internal abstract class MediaFileModel : ModelBase {
		private GpsLocation _location;
		private DateTime _creationTime;
		private DateTime _modifiedTime;
		private DateTime _lastAccessTime;
		private long? _fileSize;
		private int _rate;

		protected bool ThumbnailLoaded {
			get;
			set;
		}

		protected bool FileInfoLoaded {
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
			get;
		}

		/// <summary>
		/// 座標
		/// </summary>
		public GpsLocation Location {
			get {
				return this._location;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._location, value);
			}
		}

		/// <summary>
		/// タグリスト
		/// </summary>
		public ReactiveCollection<string> Tags {
			get;
		} = new ReactiveCollection<string>();

		/// <summary>
		/// 作成日時
		/// </summary>
		public DateTime CreationTime {
			get {
				return this._creationTime;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._creationTime, value);
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
				this.RaisePropertyChangedIfSet(ref this._modifiedTime, value);
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
				this.RaisePropertyChangedIfSet(ref this._lastAccessTime, value);
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
				this.RaisePropertyChangedIfSet(ref this._fileSize, value);
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
				this.RaisePropertyChangedIfSet(ref this._rate, value, nameof(this.Properties));
			}
		}

		/// <summary>
		/// サムネイル作成場所
		/// </summary>
		public ThumbnailLocation ThumbnailLocation {
			get;
			set;
		}

		/// <summary>
		/// プロパティ
		/// </summary>
		public virtual IEnumerable<TitleValuePair<string>> Properties {
			get {
				return new Dictionary<string, string> {
					{ "作成日時",$"{this.CreationTime}" },
					{ "編集日時",$"{this.ModifiedTime}" },
					{ "最終アクセス日時",$"{this.LastAccessTime}" },
					{ "ファイルサイズ",$"{this.FileSize}" },
					{ "評価",$"{this.Rate}" }
				}.ToTitleValuePair();
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
			this.Thumbnail = Get.Instance<ThumbnailPool>().ResolveOrRegister(this.FilePath);
		}

		/// <summary>
		/// もしまだ存在していなければ、サムネイル作成
		/// </summary>
		/// <param name="thumbnailLocation">サムネイル作成場所</param>
		public void CreateThumbnailIfNotExists() {
			if (!this.ThumbnailLoaded) {
				this.CreateThumbnail();
			} else if (!this.Thumbnail.Location.HasFlag(this.ThumbnailLocation)) {
				this.CreateThumbnail();
			}
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		public virtual void CreateThumbnail() {
			this.ThumbnailLoaded = true;
		}

		/// <summary>
		/// プロパティの内容をデータベースへ登録
		/// </summary>
		/// <returns>登録したレコード</returns>
		public virtual MediaFile RegisterToDataBase() {
			var mf = new MediaFile {
				FilePath = this.FilePath,
				ThumbnailFileName = this.Thumbnail.FileName,
				Latitude = this.Location?.Latitude,
				Longitude = this.Location?.Longitude,
				CreationTime = this.CreationTime,
				ModifiedTime = this.ModifiedTime,
				LastAccessTime = this.LastAccessTime,
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
					.Include(x => x.ImageFile)
					.Include(x => x.VideoFile)
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
			this.Thumbnail.FileName = record.ThumbnailFileName;
			if (record.Latitude is double lat && record.Longitude is double lon) {
				this.Location = new GpsLocation(lat, lon);
			} else {
				this.Location = null;
			}
			this.CreationTime = record.CreationTime;
			this.ModifiedTime = record.ModifiedTime;
			this.LastAccessTime = record.LastAccessTime;
			this.FileSize = record.FileSize;
			this.Rate = record.Rate;
			this.Tags.Clear();
			this.Tags.AddRange(record.MediaFileTags.Select(t => t.Tag.TagName));
		}

		/// <summary>
		/// まだ読み込まれていなければファイル情報読み込み
		/// </summary>
		public void GetFileInfoIfNotLoaded() {
			if (!this.FileInfoLoaded) {
				this.GetFileInfo();
			}
		}

		/// <summary>
		/// ファイル情報読み込み
		/// </summary>
		public virtual void GetFileInfo() {
			var fi = new FileInfo(this.FilePath);
			this.CreationTime = fi.CreationTime;
			this.ModifiedTime = fi.LastWriteTime;
			this.LastAccessTime = fi.LastAccessTime;
			this.FileSize = fi.Length;
			this.FileInfoLoaded = true;
			this.RaisePropertyChanged(nameof(this.Properties));
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.FilePath}>";
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
