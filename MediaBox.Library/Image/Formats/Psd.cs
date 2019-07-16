using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.Photoshop;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Psdメタデータ取得クラス
	/// </summary>
	public class Psd : ImageBase {
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

		internal Psd(Stream stream) : base(stream) {
			var reader = PsdMetadataReader.ReadMetadata(stream);
			var d = reader.First(x => x is PsdHeaderDirectory);
			this.Width = d.GetUInt16(PsdHeaderDirectory.TagImageWidth);
			this.Height = d.GetUInt16(PsdHeaderDirectory.TagImageHeight);
		}
	}
}
