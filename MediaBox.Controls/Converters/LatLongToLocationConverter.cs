using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Maps.MapControl.WPF;

namespace SandBeige.MediaBox.Controls.Converters {
	public class LatLongToLocationConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (values[0] is double latitude && values[1] is double longitude) {
				return new Location(latitude, longitude);
			}
			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			if (value is Location location) {
				return new object[] { location.Latitude, location.Longitude };
			}
			return null;
		}
	}
}
