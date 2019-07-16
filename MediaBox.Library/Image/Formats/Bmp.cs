using System.Collections.Generic;
using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.Bmp;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Bmpメタデータ取得クラス
	/// </summary>
	public class Bmp : ImageBase {
		private readonly IReadOnlyList<MetadataExtractor.Directory> _reader;
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
		internal Bmp(Stream stream) : base(stream) {
			var d = BmpMetadataReader.ReadMetadata(stream);
			this._reader = new[] { d };
			this.Width = d.GetUInt16(BmpHeaderDirectory.TagImageWidth);
			this.Height = d.GetUInt16(BmpHeaderDirectory.TagImageHeight);
		}

		public void UpdateRowdata(DataBase.Tables.Metadata.Bmp rowdata) {
			var b = this._reader.FirstOrDefault(x => x is BmpHeaderDirectory);

			rowdata.BitsPerPixel = this.GetInt(b, BmpHeaderDirectory.TagBitsPerPixel);
			rowdata.Compression = this.GetInt(b, BmpHeaderDirectory.TagCompression);
			rowdata.XPixelsPerMeter = this.GetInt(b, BmpHeaderDirectory.TagXPixelsPerMeter);
			rowdata.YPixelsPerMeter = this.GetInt(b, BmpHeaderDirectory.TagYPixelsPerMeter);
			rowdata.PaletteColorCount = this.GetInt(b, BmpHeaderDirectory.TagPaletteColourCount);
			rowdata.ImportantColorCount = this.GetInt(b, BmpHeaderDirectory.TagImportantColourCount);

		}
	}
}
