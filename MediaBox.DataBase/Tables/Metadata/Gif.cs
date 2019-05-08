namespace SandBeige.MediaBox.DataBase.Tables.Metadata {
	/// <summary>
	/// Gifメタデータテーブル
	/// </summary>
	public class Gif : MetadataBase {

		/// <summary>
		/// カラーテーブルサイズ
		/// </summary>
		public int? ColorTableSize {
			get;
			set;
		}

		/// <summary>
		/// カラーテーブルがソート済みか
		/// </summary>
		public int? IsColorTableSorted {
			get;
			set;
		}

		/// <summary>
		/// 色ビット数
		/// </summary>
		public int? BitsPerPixel {
			get;
			set;
		}

		/// <summary>
		/// グローバルカラーテーブルが存在するか
		/// </summary>
		public int? HasGlobalColorTable {
			get;
			set;
		}

		/// <summary>
		/// 背景色インデックス
		/// </summary>
		public int? BackgroundColorIndex {
			get;
			set;
		}

		/// <summary>
		/// ピクセルの縦横比
		/// </summary>
		public int? PixelAspectRatio {
			get;
			set;
		}
	}
}
