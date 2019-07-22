using System.Collections.Generic;
using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.Png;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Pngメタデータ取得クラス
	/// </summary>
	public class Png : ImageBase {
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
		internal Png(Stream stream) : base(stream) {
			this._reader = PngMetadataReader.ReadMetadata(stream);
			var d = this._reader.First(x => x is PngDirectory);
			this.Width = d.GetUInt16(PngDirectory.TagImageWidth);
			this.Height = d.GetUInt16(PngDirectory.TagImageHeight);
		}

		public void UpdateRowData(DataBase.Tables.Metadata.Png rowData) {
			var p = this._reader.FirstOrDefault(x => x is PngDirectory);
			var pc = this._reader.FirstOrDefault(x => x is PngChromaticitiesDirectory);

			rowData.BitsPerSample = this.GetInt(p, PngDirectory.TagBitsPerSample);
			rowData.ColorType = this.GetInt(p, PngDirectory.TagColorType);
			rowData.CompressionType = this.GetInt(p, PngDirectory.TagCompressionType);
			rowData.FilterMethod = this.GetInt(p, PngDirectory.TagFilterMethod);
			rowData.InterlaceMethod = this.GetInt(p, PngDirectory.TagInterlaceMethod);
			rowData.PaletteSize = this.GetInt(p, PngDirectory.TagPaletteSize);
			rowData.PaletteHasTransparency = this.GetInt(p, PngDirectory.TagPaletteHasTransparency);
			rowData.SrgbRenderingIntent = this.GetInt(p, PngDirectory.TagSrgbRenderingIntent);
			rowData.Gamma = this.GetDouble(p, PngDirectory.TagGamma);
			rowData.IccProfileName = this.GetString(p, PngDirectory.TagIccProfileName);
			rowData.LastModificationTime = this.GetDateTime(p, PngDirectory.TagLastModificationTime);
			rowData.BackgroundColor = this.GetBinary(p, PngDirectory.TagBackgroundColor);
			rowData.PixelsPerUnitX = this.GetInt(p, PngDirectory.TagPixelsPerUnitX);
			rowData.PixelsPerUnitY = this.GetInt(p, PngDirectory.TagPixelsPerUnitY);
			rowData.UnitSpecifier = this.GetInt(p, PngDirectory.TagUnitSpecifier);
			rowData.SignificantBits = this.GetInt(p, PngDirectory.TagSignificantBits);

			rowData.WhitePointX = this.GetInt(pc, PngChromaticitiesDirectory.TagWhitePointX);
			rowData.WhitePointY = this.GetInt(pc, PngChromaticitiesDirectory.TagWhitePointY);
			rowData.RedX = this.GetInt(pc, PngChromaticitiesDirectory.TagRedX);
			rowData.RedY = this.GetInt(pc, PngChromaticitiesDirectory.TagRedY);
			rowData.GreenX = this.GetInt(pc, PngChromaticitiesDirectory.TagGreenX);
			rowData.GreenY = this.GetInt(pc, PngChromaticitiesDirectory.TagGreenY);
			rowData.BlueX = this.GetInt(pc, PngChromaticitiesDirectory.TagBlueX);
			rowData.BlueY = this.GetInt(pc, PngChromaticitiesDirectory.TagBlueY);

		}
	}
}
