using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// BooleanToVisibilityConverterの逆パターン
	/// </summary>
	public class InvertBooleanToVisibilityConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is bool b) {
				return b ? Visibility.Collapsed : Visibility.Visible;
			}
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is Visibility v) {
				return v == Visibility.Collapsed;
			}
			return false;
		}
	}
}
