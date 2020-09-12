using System;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects {
	/// <summary>
	/// ファイルタイプフィルターアイテムオブジェクト
	/// </summary>
	public class MediaTypeFilterItemObject : IFilterItemObject {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				if (this.IsVideo) {
					return "動画ファイル";
				} else {
					return "画像ファイル";
				}
			}
		}

		/// <summary>
		/// 動画ファイルか否か
		/// </summary>
		public bool IsVideo {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public MediaTypeFilterItemObject() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isVideo">動画ファイルか否か</param>
		public MediaTypeFilterItemObject(bool isVideo) {
			this.IsVideo = isVideo;
		}
	}
}
