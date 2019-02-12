using System.Collections.Generic;
using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.WebP;

using SandBeige.MediaBox.Composition.Objects;

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
		/// メタデータの値と名前のペアのリストをを持つタグディレクトリのリスト
		/// </summary>
		public override IEnumerable<TitleValuePair<IEnumerable<TitleValuePair<string>>>> Properties {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">画像ファイルストリーム</param>
		internal Riff(Stream stream) : base(stream) {
			var reader = WebPMetadataReader.ReadMetadata(stream);
			this.Properties = reader.ToProperties();
			var d = reader.First(x => x is WebPDirectory);
			this.Width = d.GetUInt16(WebPDirectory.TagImageWidth);
			this.Height = d.GetUInt16(WebPDirectory.TagImageHeight);
		}
	}
}
