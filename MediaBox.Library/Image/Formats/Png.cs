using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.Png;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Pngメタデータ取得クラス
	/// </summary>
	public class Png : ImageBase {
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
		internal Png(Stream stream) : base(stream) {
			var reader = PngMetadataReader.ReadMetadata(stream);
			this.Properties = reader.ToProperties();
			var d = reader.First(x => x is PngDirectory);
			this.Width = d.GetUInt16(PngDirectory.TagImageWidth);
			this.Height = d.GetUInt16(PngDirectory.TagImageHeight);
		}
	}
}
