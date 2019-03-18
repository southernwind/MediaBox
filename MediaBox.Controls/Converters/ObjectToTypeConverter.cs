using System;
using System.Globalization;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// 型に変換するコンバーター
	/// </summary>
	public class ObjectToTypeConverter : IValueConverter {
		/// <summary>
		/// コンバーター
		/// </summary>
		/// <param name="value">型に変換する値</param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>型</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value?.GetType();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
