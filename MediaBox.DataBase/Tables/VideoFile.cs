using System.Collections.Generic;

using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// 動画ファイルテーブル
	/// </summary>
	public class VideoFile {
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

		public ICollection<VideoMetadataValue> VideoMetadataValues {
			get;
			set;
		}
	}
}
