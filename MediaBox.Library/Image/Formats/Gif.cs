using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.Gif;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Gifメタデータ取得クラス
	/// </summary>
	public class Gif : ImageBase {
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
		/// メタデータの値と名前のペアのリストをを持つタグディレクトリのリスト
		/// </summary>
		public override Attributes<Attributes<string>> Properties {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">画像ファイルストリーム</param>
		internal Gif(Stream stream) : base(stream) {
			var reader = GifMetadataReader.ReadMetadata(stream);
			this.Properties = reader.ToProperties();
			var d = reader.First(x => x is GifHeaderDirectory);
			this.Width = d.GetUInt16(GifHeaderDirectory.TagImageWidth);
			this.Height = d.GetUInt16(GifHeaderDirectory.TagImageHeight);
		}
	}
}
