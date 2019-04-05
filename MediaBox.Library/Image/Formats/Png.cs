using System;
using System.Collections.Generic;
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
			this._reader = PngMetadataReader.ReadMetadata(stream);
			this.Properties = this._reader.ToProperties();
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

		/// <summary>
		/// 数値取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		private int? GetInt(MetadataExtractor.Directory directory, int tag) {
			if (directory.TryGetInt32(tag, out var value)) {
				return value;
			}
			return default;
		}

		/// <summary>
		/// 数値取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		private double? GetDouble(MetadataExtractor.Directory directory, int tag) {
			if (directory.TryGetDouble(tag, out var value)) {
				return value;
			}
			return default;
		}

		/// <summary>
		/// 日付取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		private DateTime? GetDateTime(MetadataExtractor.Directory directory, int tag) {
			if (directory.TryGetDateTime(tag, out var value)) {
				return value;
			}
			return default;
		}

		/// <summary>
		/// 文字取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		private string GetString(MetadataExtractor.Directory directory, int tag) {
			return directory.GetString(tag);
		}

		/// <summary>
		/// 文字型取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		private byte[] GetBinary(MetadataExtractor.Directory directory, int tag) {
			return directory.GetByteArray(tag);
		}
	}
}
