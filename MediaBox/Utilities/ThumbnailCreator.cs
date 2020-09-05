using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

using SandBeige.MediaBox.Library.Creator;

namespace SandBeige.MediaBox.Utilities {
	/// <summary>
	/// サムネイル作成クラス
	/// </summary>
	public static class ThumbnailCreator {
		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="image">元画像</param>
		/// <param name="width">縮小後最大幅</param>
		/// <param name="height">縮小後最大高さ</param>
		/// <param name="orientation">画像反転/回転</param>
		/// <returns>サムネイル画像</returns>
		public static Image Create(Image image, int width, int height, int? orientation) {
			image.RotateFlip(GetRotateFlipType(orientation));
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
		/// <param name="orientation">画像反転/回転</param>
		/// <returns>サムネイル画像</returns>
		public static byte[] Create(Stream imageStream, int width, int height, int? orientation) {
			using var ms = new MemoryStream();
			Create(Image.FromStream(imageStream), width, height, orientation).Save(ms, ImageFormat.Jpeg);
			return ms.ToArray();
		}

		/// <summary>
		/// Orientation→画像反転/回転変換
		/// </summary>
		/// <param name="orientation">Orientation</param>
		/// <returns>画像反転/回転</returns>
		private static RotateFlipType GetRotateFlipType(int? orientation) {
			var rotation = ImageSourceCreator.GetRotation(orientation);
			var needsFlipX = ImageSourceCreator.NeedsFlipX(orientation);

			return rotation switch
			{
				Rotation.Rotate90 => needsFlipX ? RotateFlipType.Rotate90FlipX : RotateFlipType.Rotate90FlipNone,
				Rotation.Rotate180 => needsFlipX ? RotateFlipType.Rotate180FlipX : RotateFlipType.Rotate180FlipNone,
				Rotation.Rotate270 => needsFlipX ? RotateFlipType.Rotate270FlipX : RotateFlipType.Rotate270FlipNone,
				_ => needsFlipX ? RotateFlipType.RotateNoneFlipX : RotateFlipType.RotateNoneFlipNone,
			};
		}
	}
}
