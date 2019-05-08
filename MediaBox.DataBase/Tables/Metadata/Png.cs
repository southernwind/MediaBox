using System;

namespace SandBeige.MediaBox.DataBase.Tables.Metadata {
	/// <summary>
	/// Pngメタデータテーブル
	/// </summary>
	public class Png : MetadataBase {
		/// <summary>
		/// サンプルあたりのビット数
		/// </summary>
		public int? BitsPerSample {
			get;
			set;
		}

		/// <summary>
		/// 画像種類
		/// 0 : グレースケール
		/// 2 : トゥルーカラー
		/// 3 : インデックスカラー
		/// 4 : アルファ付きグレースケール
		/// 6 : アルファ付きトゥルーカラー
		/// </summary>
		public int? ColorType {
			get;
			set;
		}

		/// <summary>
		/// 圧縮種類
		/// </summary>
		public int? CompressionType {
			get;
			set;
		}

		/// <summary>
		/// フィルター方式
		/// </summary>
		public int? FilterMethod {
			get;
			set;
		}

		/// <summary>
		/// 画像送信順序
		/// </summary>
		public int? InterlaceMethod {
			get;
			set;
		}

		/// <summary>
		/// パレット数
		/// </summary>
		public int? PaletteSize {
			get;
			set;
		}

		/// <summary>
		/// 透過Pngか否か
		/// </summary>
		public int? PaletteHasTransparency {
			get;
			set;
		}

		/// <summary>
		/// レンダリングインテント
		/// 0 : 知覚
		/// 1 : 相対カラーメトリック
		/// 2 : 彩度
		/// 3 : 絶対カラーメトリック
		/// </summary>
		public int? SrgbRenderingIntent {
			get;
			set;
		}

		/// <summary>
		/// ガンマ値
		/// </summary>
		public double? Gamma {
			get;
			set;
		}

		/// <summary>
		/// プロファイル名
		/// </summary>
		public string IccProfileName {
			get;
			set;
		}

		/// <summary>
		/// 最終修正日
		/// </summary>
		public DateTime? LastModificationTime {
			get;
			set;
		}

		/// <summary>
		/// 背景色
		/// </summary>
		public byte[] BackgroundColor {
			get;
			set;
		}

		/// <summary>
		/// 単位あたりのピクセル数 X
		/// </summary>
		public int? PixelsPerUnitX {
			get;
			set;
		}

		/// <summary>
		/// 単位あたりのピクセル数 Y
		/// </summary>
		public int? PixelsPerUnitY {
			get;
			set;
		}

		/// <summary>
		/// 単位
		/// 0 : 指定なし
		/// 1 : メートル
		/// </summary>
		public int? UnitSpecifier {
			get;
			set;
		}

		/// <summary>
		/// 有効ビット数
		/// </summary>
		public int? SignificantBits {
			get;
			set;
		}

		/// <summary>
		/// 基礎色度 白X
		/// </summary>
		public int? WhitePointX {
			get;
			set;
		}

		/// <summary>
		/// 基礎色度 白Y
		/// </summary>
		public int? WhitePointY {
			get;
			set;
		}

		/// <summary>
		/// 基礎色度 赤X
		/// </summary>
		public int? RedX {
			get;
			set;
		}

		/// <summary>
		/// 基礎色度 赤Y
		/// </summary>
		public int? RedY {
			get;
			set;
		}

		/// <summary>
		/// 基礎色度 緑X
		/// </summary>
		public int? GreenX {
			get;
			set;
		}

		/// <summary>
		/// 基礎色度 緑Y
		/// </summary>
		public int? GreenY {
			get;
			set;
		}

		/// <summary>
		/// 基礎色度 青X
		/// </summary>
		public int? BlueX {
			get;
			set;
		}

		/// <summary>
		/// 基礎色度 青Y
		/// </summary>
		public int? BlueY {
			get;
			set;
		}

	}
}
