using System;
using System.Globalization;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.Converters {
	public class NullableConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
				value is string text &&
				string.IsNullOrEmpty(text)) {
				return null;
			}
			return value;
		}
	}
}
