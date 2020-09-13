using System.IO;

using ImageMagick;

namespace SandBeige.MediaBox.Library.Image {
	public class ThumbnailCreator {
		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="imageStream">元画像</param>
		/// <param name="width">縮小後最大幅</param>
		/// <param name="height">縮小後最大高さ</param>
		/// <param name="orientation">画像反転/回転</param>
		/// <returns>サムネイル画像</returns>
		public static byte[] Create(Stream imageStream, int width, int height) {
			using var ms = new MemoryStream();
			using var mi = new MagickImage(imageStream);
			mi.AdaptiveResize(width, height);
			mi.Format = MagickFormat.Jpg;
			mi.Write(ms);
			return ms.ToArray();
		}
	}
}
