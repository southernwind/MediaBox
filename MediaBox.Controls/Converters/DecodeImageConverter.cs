using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

using SandBeige.MediaBox.Library.Creator;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// パスとサイズからリサイズ後画像に変換
	/// </summary>
	public class DecodeImageConverter : IMultiValueConverter {
		/// <summary>
		/// パス & サイズ→リサイズ後画像
		/// </summary>
		/// <param name="values">
		/// <list>
		/// <item>[0] = ファイルパス(string)</item>
		/// <item>[1] = 画像の回転(int?)</item>
		/// <item>[2] = 幅(double)</item>
		/// <item>[3] = 高さ(double)</item>
		/// </list>
		/// </param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns></returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (!(values[0] is string) && !(values[0] is Stream)) {
				return null;
			}
			var orientation = values[1] as int?;
			if (values.Length == 4 && values[2] is double width && values[3] is double height) {
				return ImageSourceCreator.Create(values[0], orientation, width, height);
			}

			return ImageSourceCreator.Create(values[0], orientation);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
