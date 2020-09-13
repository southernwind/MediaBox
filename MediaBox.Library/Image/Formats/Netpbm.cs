using System.IO;

using MetadataExtractor;
using MetadataExtractor.Formats.Netpbm;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Netpbmメタデータ取得クラス
	/// </summary>
	public class Netpbm : ImageBase {
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
		internal Netpbm(Stream stream) : base(stream) {
			var d = NetpbmMetadataReader.ReadMetadata(stream);
			this.Width = d.GetUInt16(NetpbmHeaderDirectory.TagWidth);
			this.Height = d.GetUInt16(NetpbmHeaderDirectory.TagHeight);
		}
	}
}
