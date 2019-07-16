using System.IO;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Tiffメタデータ取得クラス
	/// </summary>
	public class Tiff : ImageBase {
		/// <summary>
		/// 幅
		/// </summary>
		public override int Width {
			get;
		}

		/// <summary>
		/// 高さ
		/// </summary>
		public override int Height {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">画像ファイルストリーム</param>
		internal Tiff(Stream stream) : base(stream) {
		}
	}
}
