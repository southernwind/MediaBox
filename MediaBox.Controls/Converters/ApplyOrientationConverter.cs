using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SandBeige.MediaBox.Controls.Converters {
	public class ApplyOrientationConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			var path = values[0] as string;
			var stream = values[0] as Stream;
			if ((path ?? (object)stream) == null) {
				return null;
			}

			var orientation = values[1] as int?;
			var image = new BitmapImage();
			image.BeginInit();
			if (path != null) {
				image.UriSource = new Uri(path);
			} else {
				image.StreamSource = stream;
			}
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.CreateOptions = BitmapCreateOptions.None;
			switch (orientation) {
				case null:
				case 1:
				case 2:
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

			if (new int?[] { 2, 4, 5, 7 }.Contains(orientation)) {
				return new TransformedBitmap(image, new ScaleTransform(-1, 1, 0, 0));
			}
			return image;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
