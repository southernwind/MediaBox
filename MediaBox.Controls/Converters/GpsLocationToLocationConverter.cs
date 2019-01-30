using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using Microsoft.Maps.MapControl.WPF;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Controls.Converters {
	public class GpsLocationToLocationConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is GpsLocation location) {
				return new Location(location.Latitude, location.Longitude);
			}
			if (value == DependencyProperty.UnsetValue) {
				return DependencyProperty.UnsetValue;
			}
			return null;
		}

		public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture) {
			if (value is Location location) {
				return new GpsLocation(location.Latitude, location.Longitude);
			}
			return null;
		}
	}
}
