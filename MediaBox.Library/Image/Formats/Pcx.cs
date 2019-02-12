using System.Collections.Generic;
using System.IO;

using MetadataExtractor;
using MetadataExtractor.Formats.Pcx;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Pcxメタデータ取得クラス
	/// </summary>
	public class Pcx : ImageBase {
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

		internal Pcx(Stream stream) : base(stream) {
			var d = PcxMetadataReader.ReadMetadata(stream);
			var reader = new[] { d };
			this.Properties = reader.ToProperties();
			var xStart = d.GetUInt16(PcxDirectory.TagXMin);
			var xEnd = d.GetUInt16(PcxDirectory.TagXMax);
			var yStart = d.GetUInt16(PcxDirectory.TagYMin);
			var yEnd = d.GetUInt16(PcxDirectory.TagYMax);
			this.Width = xEnd - xStart + 1;
			this.Height = yEnd - yStart + 1;
		}
	}
}
