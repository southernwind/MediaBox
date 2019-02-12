using System.Collections.Generic;
using System.IO;

using MetadataExtractor;
using MetadataExtractor.Formats.Bmp;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Bmpメタデータ取得クラス
	/// </summary>
	public class Bmp : ImageBase {
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
		public override IEnumerable<TitleValuePair<IEnumerable<TitleValuePair<string>>>> Properties {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">画像ファイルストリーム</param>
		internal Bmp(Stream stream) : base(stream) {
			var d = BmpMetadataReader.ReadMetadata(stream);
			var reader = new[] { d };
			this.Properties = reader.ToProperties();
			this.Width = d.GetUInt16(BmpHeaderDirectory.TagImageWidth);
			this.Height = d.GetUInt16(BmpHeaderDirectory.TagImageHeight);
		}
	}
}
