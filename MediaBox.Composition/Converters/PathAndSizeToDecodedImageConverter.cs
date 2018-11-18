using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SandBeige.MediaBox.Composition.Converters {
	/// <summary>
	/// パスとサイズからリサイズ後画像に変換
	/// </summary>
	public class PathAndSizeToDecodedImageConverter : IMultiValueConverter {
		/// <summary>
		/// パス&サイズ→リサイズ後画像
		/// </summary>
		/// <param name="values">
		/// <list>
		/// <item>[0] = ファイルパス(string)</item>
		/// <item>[1] = 幅(double)</item>
		/// <item>[2] = 高さ(double)</item>
		/// </list>
		/// </param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns></returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(values[0] is string path && values[1] is double width && values[2] is double height) {
				var image = new BitmapImage();
				image.BeginInit();
				image.UriSource = new Uri(path);
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.CreateOptions = BitmapCreateOptions.None;
				image.DecodePixelWidth = (int)width;
				image.DecodePixelHeight = (int)height;
				image.EndInit();
				image.Freeze();
				return image;
			}
			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
