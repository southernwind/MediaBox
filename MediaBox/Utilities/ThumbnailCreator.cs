using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SandBeige.MediaBox.Utilities {
	internal class ThumbnailCreator {
		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="image">元画像</param>
		/// <param name="width">縮小後最大幅</param>
		/// <param name="height">縮小後最大高さ</param>
		/// <returns>サムネイル画像</returns>
		public static Image Create(Image image, int width, int height) {
			if (image == null) {
				return null;
			}
			var scale = Math.Min((double)width / image.Width, (double)height / image.Height);
			if (scale >= 1) {
				return image;
			}

			var w = (int)Math.Round(image.Width * scale);
			var h = (int)Math.Round(image.Height * scale);

			return image.GetThumbnailImage(w, h, () => false, IntPtr.Zero);
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="imageStream">元画像</param>
		/// <param name="width">縮小後最大幅</param>
		/// <param name="height">縮小後最大高さ</param>
		/// <returns>サムネイル画像</returns>
		public static byte[] Create(Stream imageStream, int width, int height) {
			if (imageStream == null) {
				return null;
			}
			using (var ms = new MemoryStream()) {
				Create(Image.FromStream(imageStream), width, height).Save(ms, ImageFormat.Jpeg);
				return ms.ToArray();
			}
		}
	}
}
