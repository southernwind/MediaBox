using System.Collections.Generic;
using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.Gif;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Gifメタデータ取得クラス
	/// </summary>
	public class Gif : ImageBase {
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
		internal Gif(Stream stream) : base(stream) {
			this._reader = GifMetadataReader.ReadMetadata(stream);
			var d = this._reader.First(x => x is GifHeaderDirectory);
			this.Width = d.GetUInt16(GifHeaderDirectory.TagImageWidth);
			this.Height = d.GetUInt16(GifHeaderDirectory.TagImageHeight);
		}

		public void UpdateRowData(DataBase.Tables.Metadata.Gif rowData) {
			var h = this._reader.FirstOrDefault(x => x is GifHeaderDirectory);

			rowData.ColorTableSize = this.GetInt(h, GifHeaderDirectory.TagColorTableSize);
			rowData.IsColorTableSorted = this.GetInt(h, GifHeaderDirectory.TagIsColorTableSorted);
			rowData.BitsPerPixel = this.GetInt(h, GifHeaderDirectory.TagBitsPerPixel);
			rowData.HasGlobalColorTable = this.GetInt(h, GifHeaderDirectory.TagHasGlobalColorTable);
			rowData.BackgroundColorIndex = this.GetInt(h, GifHeaderDirectory.TagBackgroundColorIndex);
			rowData.PixelAspectRatio = this.GetInt(h, GifHeaderDirectory.TagPixelAspectRatio);
		}
	}
}
