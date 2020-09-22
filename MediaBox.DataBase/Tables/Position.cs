using System;
using System.Collections.Generic;

namespace SandBeige.MediaBox.DataBase.Tables {
	public class Position {
		private string? _displayName;
		private ICollection<PositionAddress>? _addresses;
		private ICollection<PositionNameDetail>? _nameDetails;
		private ICollection<MediaFile>? _mediaFiles;

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
			get {
				return this._displayName ?? throw new InvalidOperationException();
			}
			set {
				this._displayName = value;
			}
		}

		/// <summary>
		/// アドレス
		/// </summary>
		public virtual ICollection<PositionAddress> Addresses {
			get {
				return this._addresses ?? throw new InvalidOperationException();
			}
			set {
				this._addresses = value;
			}
		}

		/// <summary>
		/// 別名リスト
		/// </summary>
		public virtual ICollection<PositionNameDetail> NameDetails {
			get {
				return this._nameDetails ?? throw new InvalidOperationException();
			}
			set {
				this._nameDetails = value;
			}
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
		/// データ取得済みか否か
		/// </summary>
		public bool IsAcquired {
			get;
			set;
		}

		/// <summary>
		/// メディアファイル
		/// </summary>
		public ICollection<MediaFile> MediaFiles {
			get {
				return this._mediaFiles ?? throw new InvalidOperationException();
			}
			set {
				this._mediaFiles = value;
			}
		}
	}
}
