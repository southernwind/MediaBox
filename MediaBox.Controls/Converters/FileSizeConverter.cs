using System;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.Converters {
	public class FileSizeConverter : IValueConverter {
		private static readonly string[] _suffix = { "", "K", "M", "G", "T" };
		private const double _unit = 1024;

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

			var size = (double)(long)value;
			var i = 0;
			for (; i < _suffix.Length - 1; i++) {
				if (size < _unit) {
					break;
				}
				size /= _unit;
			}

			return $"{size:0.00} {_suffix[i]}B";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

	}
}
