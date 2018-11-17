using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBeige.MediaBox.Utilities {
	class ThumbnailCreator {
		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="image">元画像</param>
		/// <param name="width">縮小後最大幅</param>
		/// <param name="height">縮小後最大高さ</param>
		/// <returns>サムネイル画像</returns>
		public static Image Create(Image image, int width, int height) {
			if (image == null) {
				return image;
			}
			var scale = Math.Min((double)width / image.Width, height / image.Height);
			if (scale >= 1) {
				return image;
			}

			var w = (int)Math.Round(image.Width * scale);
			var h = (int)Math.Round(image.Height * scale);

			return image.GetThumbnailImage(w, h, () => { return false; }, IntPtr.Zero);
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="image">元画像</param>
		/// <param name="width">縮小後最大幅</param>
		/// <param name="height">縮小後最大高さ</param>
		/// <returns>サムネイル画像</returns>
		public static IEnumerable<byte> Create(IEnumerable<byte> image, int width, int height) {
			if (image == null || !image.Any()) {
				return image;
			}
			using (var ms = new MemoryStream(image.ToArray()))
			using (var thms = new MemoryStream()) {
				Create(Image.FromStream(ms), width, height).Save(thms, ImageFormat.Jpeg);
				return thms.ToArray();
			}
		}
	}
}
