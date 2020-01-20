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
			this._reader = BmpMetadataReader.ReadMetadata(stream);
			var d = this._reader.First(x => x is BmpHeaderDirectory);
			this.Width = d.GetUInt16(BmpHeaderDirectory.TagImageWidth);
			this.Height = d.GetUInt16(BmpHeaderDirectory.TagImageHeight);
		}

		public void UpdateRowData(DataBase.Tables.Metadata.Bmp rowData) {
			var b = this._reader.FirstOrDefault(x => x is BmpHeaderDirectory);

			rowData.BitsPerPixel = this.GetInt(b, BmpHeaderDirectory.TagBitsPerPixel);
			rowData.Compression = this.GetInt(b, BmpHeaderDirectory.TagCompression);
			rowData.XPixelsPerMeter = this.GetInt(b, BmpHeaderDirectory.TagXPixelsPerMeter);
			rowData.YPixelsPerMeter = this.GetInt(b, BmpHeaderDirectory.TagYPixelsPerMeter);
			rowData.PaletteColorCount = this.GetInt(b, BmpHeaderDirectory.TagPaletteColourCount);
			rowData.ImportantColorCount = this.GetInt(b, BmpHeaderDirectory.TagImportantColourCount);

		}
	}
}
