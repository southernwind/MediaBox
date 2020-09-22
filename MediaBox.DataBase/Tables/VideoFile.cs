using System;
using System.Collections.Generic;

using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// 動画ファイルテーブル
	/// </summary>
	public class VideoFile {
		private ICollection<VideoMetadataValue>? _videoMetadataValues;
		private MediaFile? _mediaFile;

		/// <summary>
		/// メディアファイルID
		/// </summary>
		public long MediaFileId {
			get;
			set;
		}

		/// <summary>
		/// メディアファイル
		/// </summary>
		public MediaFile MediaFile {
			get {
				return this._mediaFile ?? throw new InvalidOperationException();
			}
			set {
				this._mediaFile = value;
			}
		}

		/// <summary>
		/// 動画の長さ
		/// </summary>
		public double? Duration {
			get;
			set;
		}

		/// <summary>
		/// 回転
		/// </summary>
		public int? Rotation {
			get;
			set;
		}

		public virtual ICollection<VideoMetadataValue> VideoMetadataValues {
			get {
				return this._videoMetadataValues ?? throw new InvalidOperationException();
			}
			set {
				this._videoMetadataValues = value;
			}
		}
	}
}
