using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.WebP;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Riffメタデータ取得クラス
	/// </summary>
	public class Riff : ImageBase {
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
		internal Riff(Stream stream) : base(stream) {
			var reader = WebPMetadataReader.ReadMetadata(stream);
			var d = reader.First(x => x is WebPDirectory);
			this.Width = d.GetUInt16(WebPDirectory.TagImageWidth);
			this.Height = d.GetUInt16(WebPDirectory.TagImageHeight);
		}
	}
}
