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

		public void UpdateRowdata(DataBase.Tables.Metadata.Png rowdata) {
			var p = this._reader.FirstOrDefault(x => x is PngDirectory);
			var pc = this._reader.FirstOrDefault(x => x is PngChromaticitiesDirectory);

			if (p != null) {
				rowdata.BitsPerSample = this.GetInt(p, PngDirectory.TagBitsPerSample);
				rowdata.ColorType = this.GetInt(p, PngDirectory.TagColorType);
				rowdata.CompressionType = this.GetInt(p, PngDirectory.TagCompressionType);
				rowdata.FilterMethod = this.GetInt(p, PngDirectory.TagFilterMethod);
				rowdata.InterlaceMethod = this.GetInt(p, PngDirectory.TagInterlaceMethod);
				rowdata.PaletteSize = this.GetInt(p, PngDirectory.TagPaletteSize);
				rowdata.PaletteHasTransparency = this.GetInt(p, PngDirectory.TagPaletteHasTransparency);
				rowdata.SrgbRenderingIntent = this.GetInt(p, PngDirectory.TagSrgbRenderingIntent);
				rowdata.Gamma = this.GetDouble(p, PngDirectory.TagGamma);
				rowdata.IccProfileName = this.GetString(p, PngDirectory.TagIccProfileName);
				rowdata.LastModificationTime = this.GetDateTime(p, PngDirectory.TagLastModificationTime);
				rowdata.BackgroundColor = this.GetBinary(p, PngDirectory.TagBackgroundColor);
				rowdata.PixelsPerUnitX = this.GetInt(p, PngDirectory.TagPixelsPerUnitX);
				rowdata.PixelsPerUnitY = this.GetInt(p, PngDirectory.TagPixelsPerUnitY);
				rowdata.UnitSpecifier = this.GetInt(p, PngDirectory.TagUnitSpecifier);
				rowdata.SignificantBits = this.GetInt(p, PngDirectory.TagSignificantBits);
			}

			if (pc != null) {
				rowdata.WhitePointX = this.GetInt(pc, PngChromaticitiesDirectory.TagWhitePointX);
				rowdata.WhitePointY = this.GetInt(pc, PngChromaticitiesDirectory.TagWhitePointY);
				rowdata.RedX = this.GetInt(pc, PngChromaticitiesDirectory.TagRedX);
				rowdata.RedY = this.GetInt(pc, PngChromaticitiesDirectory.TagRedY);
				rowdata.GreenX = this.GetInt(pc, PngChromaticitiesDirectory.TagGreenX);
				rowdata.GreenY = this.GetInt(pc, PngChromaticitiesDirectory.TagGreenY);
				rowdata.BlueX = this.GetInt(pc, PngChromaticitiesDirectory.TagBlueX);
				rowdata.BlueY = this.GetInt(pc, PngChromaticitiesDirectory.TagBlueY);
			}
		}
	}
}
