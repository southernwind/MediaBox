using System.Collections.Generic;
using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.Ico;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Icoメタデータ取得クラス
	/// </summary>
	public class Ico : ImageBase {
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

		internal Ico(Stream stream) : base(stream) {
			var reader = IcoMetadataReader.ReadMetadata(stream);
			this.Properties = reader.ToProperties();
			var d = reader.First(x => x is IcoDirectory);
			this.Width = d.GetUInt16(IcoDirectory.TagImageWidth);
			this.Height = d.GetUInt16(IcoDirectory.TagImageHeight);
		}
	}
}
