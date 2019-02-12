using System;
using System.IO;

using MetadataExtractor.Util;

using SandBeige.MediaBox.Library.Image.Formats;

namespace SandBeige.MediaBox.Library.Image {
	/// <summary>
	/// 画像メタデータクラス
	/// </summary>
	public static class ImageMetadataFactory {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">画像ファイルストリーム</param>
		public static IImage Create(Stream stream) {
			switch (FileTypeDetector.DetectFileType(stream)) {
				case FileType.Jpeg:
					return new Jpeg(stream);
				case FileType.Tiff:
				case FileType.Arw:
				case FileType.Cr2:
				case FileType.Nef:
				case FileType.Orf:
				case FileType.Rw2:
					// TODO : return new Tiff(stream);
					throw new ArgumentException("未対応ファイル形式");
				case FileType.Psd:
					return new Psd(stream);
				case FileType.Png:
					return new Png(stream);
				case FileType.Bmp:
					return new Bmp(stream);
				case FileType.Gif:
					return new Gif(stream);
				case FileType.Ico:
					return new Ico(stream);
				case FileType.Pcx:
					return new Pcx(stream);
				case FileType.Riff:
					return new Riff(stream);
				case FileType.Raf:
					return new Raf(stream);
				case FileType.QuickTime:
					throw new ArgumentException("未対応ファイル形式");
				case FileType.Netpbm:
					return new Netpbm(stream);
				default:
					throw new ArgumentException("未対応ファイル形式");
			}
		}
	}
}
