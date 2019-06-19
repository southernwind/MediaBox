using System.Collections.Generic;

namespace SandBeige.MediaBox.DataBase.Tables {
	public class Position {
		/// <summary>
		/// 緯度
		/// </summary>
		public double Latitude {
			get;
			set;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public double Longitude {
			get;
			set;
		}

		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get;
			set;
		}

		/// <summary>
		/// アドレス
		/// </summary>
		public ICollection<PositionAddress> Addresses {
			get;
			set;
		}

		/// <summary>
		/// 別名リスト
		/// </summary>
		public ICollection<PositionNameDetail> NameDetails {
			get;
			set;
		}

		/// <summary>
		/// 地形を囲う矩形の座標 (左)
		/// </summary>
		public double BoundingBoxLeft {
			get;
			set;
		}

		/// <summary>
		/// 地形を囲う矩形の座標 (右)
		/// </summary>
		public double BoundingBoxRight {
			get;
			set;
		}

		/// <summary>
		/// 地形を囲う矩形の座標 (上)
		/// </summary>
		public double BoundingBoxTop {
			get;
			set;
		}

		/// <summary>
		/// 地形を囲う矩形の座標 (下)
		/// </summary>
		public double BoundingBoxBottom {
			get;
			set;
		}

		/// <summary>
		/// メディアファイル
		/// </summary>
		public ICollection<MediaFile> MediaFiles {
			get;
			set;
		}
	}
}
