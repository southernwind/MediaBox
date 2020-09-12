using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

using ImageMagick;

using SandBeige.MediaBox.Library.Creator;

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
