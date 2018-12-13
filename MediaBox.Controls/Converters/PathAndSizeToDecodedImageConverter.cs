using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// パスとサイズからリサイズ後画像に変換
	/// </summary>
	public class PathAndSizeToDecodedImageConverter : IMultiValueConverter {
		/// <summary>
		/// パス & サイズ→リサイズ後画像
		/// </summary>
		/// <param name="values">
		/// <list>
		/// <item>[0] = ファイルパス(string)</item>
		/// <item>[1] = 幅(double)</item>
		/// <item>[2] = 高さ(double)</item>
		/// <item>[3] = 画像の回転(int?)</item>
		/// </list>
		/// </param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns></returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (!(values[0] is string path) || !(values[1] is double width) || !(values[2] is double height)) {
				return null;
			}

			var orientation = values[3] as int?;
			var image = new BitmapImage();
			image.BeginInit();
			image.UriSource = new Uri(path);
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.CreateOptions = BitmapCreateOptions.None;
			image.DecodePixelWidth = (int)width;
			image.DecodePixelHeight = (int)height;
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
