using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SandBeige.MediaBox.Library.Creator {
	public static class ImageSourceCreator {
		public static ImageSource Create(object source, int? orientation = null, double width = 0, double height = 0) {
			var image = new BitmapImage();
			image.BeginInit();
			switch (source)
			{
				case string path:
					image.UriSource = new Uri(path);
					break;
				case Uri uri:
					image.UriSource = uri;
					break;
				case Stream stream:
					image.StreamSource = stream;
					break;
				default:
					throw new ArgumentException();
			}

			image.CacheOption = BitmapCacheOption.OnLoad;
			image.CreateOptions = BitmapCreateOptions.None;

			image.DecodePixelWidth = (int)width;
			image.DecodePixelHeight = (int)height;

			switch (orientation) {
				default:
					image.Rotation = Rotation.Rotate0;
					break;
				case 3:
				case 4:
					image.Rotation = Rotation.Rotate180;
					break;
				case 5:
				case 8:
					image.Rotation = Rotation.Rotate270;
					break;
				case 6:
				case 7:
					image.Rotation = Rotation.Rotate90;
					break;
			}

			image.EndInit();
			image.Freeze();

			// 反転させる必要がないならそのまま
			if (!new int?[] {2, 4, 5, 7}.Contains(orientation)) {
				return image;
			}

			// 反転
			var tb = new TransformedBitmap(image, new ScaleTransform(-1, 1, 0, 0));
			tb.Freeze();
			return tb;

		}
	}
}
