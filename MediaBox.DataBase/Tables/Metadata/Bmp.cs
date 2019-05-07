﻿namespace SandBeige.MediaBox.DataBase.Tables.Metadata {
	public class Bmp {
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
			get;
			set;
		}

		/// <summary>
		/// 色ビット数
		/// </summary>
		public int BitsPerPixel {
			get;
			set;
		}

		/// <summary>
		/// 圧縮形式
		/// </summary>
		public int Compression {
			get;
			set;
		}

		/// <summary>
		/// 水平解像度
		/// </summary>
		public int XPixelsPerMeter {
			get;
			set;
		}

		/// <summary>
		/// 垂直解像度
		/// </summary>
		public int YPixelsPerMeter {
			get;
			set;
		}

		/// <summary>
		/// 使用色数
		/// </summary>
		public int PaletteColorCount {
			get;
			set;
		}

		/// <summary>
		/// 重要色数
		/// </summary>
		public int ImportantColorCount {
			get;
			set;
		}
	}
}
