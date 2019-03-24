using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルクラス
	/// </summary>
	/// <remarks>
	/// メディア間共通プロパティの定義と取得、登録を行う。
	/// </remarks>
	internal abstract class MediaFileModel : ModelBase, IMediaFileModel {
		private ComparableSize? _resolution;
		private GpsLocation _location;
		private DateTime _creationTime;
		private DateTime _modifiedTime;
		private DateTime _lastAccessTime;
		private long? _fileSize;
		private IThumbnail _thumbnail;
		private int _rate;
		private bool _isInvalid;
		private Attributes<Attributes<string>> _metadata;

		/// <summary>
		/// データベースから情報を取得済みか
		/// </summary>
		protected bool LoadedFromDataBase {
			get;
			set;
		}

		/// <summary>
		/// サムネイルが読み込み済みか
		/// </summary>
		protected bool ThumbnailLoaded {
			get;
			set;
		}

		/// <summary>
		/// ファイル情報取得済みか
		/// </summary>
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
		/// サムネイル
		/// </summary>
		public IThumbnail Thumbnail {
			get {
				return this._thumbnail;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._thumbnail, value);
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
				this.RaisePropertyChangedIfSet(ref this._resolution, value, nameof(this.Properties));
			}
		}

		/// <summary>
		/// 座標
		/// </summary>
		public GpsLocation Location {
			get {
				return this._location;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._location, value, nameof(this.Properties));
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
		/// 不正なファイル
		/// </summary>
		public bool IsInvalid {
			get {
				return this._isInvalid;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._isInvalid, value);
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
					{ "解像度" , this.Resolution?.ToString() }
				}.ToAttributes();
			}
		}

		/// <summary>
		/// メディアファイルのメタデータ
		/// </summary>
		/// <remarks>
		/// - メタデータグループ1
		///		- メタデータ1(タイトル:値)
		///		- メタデータ2(タイトル:値)
		///		- メタデータ3(タイトル:値)
		/// - メタデータグループ2
		///		-メタデータ1(タイトル:値)
		///		- メタデータ2(タイトル:値)
		/// ...という感じで値が入る
		/// 形式さえ合わせれば具象クラスでどんなデータを入れてもOK
		/// </remarks>
		public virtual Attributes<Attributes<string>> Metadata {
			get {
				return this._metadata;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._metadata, value);
			}
		}

		/// <summary>
		/// コンストラクタ
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
		public void CreateThumbnailIfNotExists() {
			if (!this.ThumbnailLoaded) {
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
			if (!this.LoadedFromDataBase) {
				this.FileSize = fi.Length;
			}
			this.FileInfoLoaded = true;
			this.RaisePropertyChanged(nameof(this.Properties));
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
			this.Thumbnail = Get.Instance<IThumbnail>(record.ThumbnailFileName);
			if (record.Latitude is double lat && record.Longitude is double lon) {
				this.Location = new GpsLocation(lat, lon);
			} else {
				this.Location = null;
			}
			this.FileSize = record.FileSize;
			this.Rate = record.Rate;
			this.Resolution = new ComparableSize(record.Width, record.Height);
			this.Tags.Clear();
			this.Tags.AddRange(record.MediaFileTags.Select(t => t.Tag.TagName));
			this.RaisePropertyChanged(nameof(this.Tags));
			this.LoadedFromDataBase = true;
		}

		/// <summary>
		/// プロパティの内容をデータベースへ登録
		/// </summary>
		/// <returns>登録したレコード</returns>
		public virtual MediaFile RegisterToDataBase() {
			var mf = new MediaFile {
				FilePath = this.FilePath,
				ThumbnailFileName = this.Thumbnail?.FileName,
				Latitude = this.Location?.Latitude,
				Longitude = this.Location?.Longitude,
				DirectoryPath = $@"{Path.GetDirectoryName(this.FilePath)}\",
				FileSize = this.FileSize,
				Rate = this.Rate,
				Width = (int)this.Resolution.Value.Width,
				Height = (int)this.Resolution.Value.Height
			};
			lock (this.DataBase) {
				this.DataBase.MediaFiles.Add(mf);
				this.DataBase.SaveChanges();
			}
			this.MediaFileId = mf.MediaFileId;
			return mf;
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
