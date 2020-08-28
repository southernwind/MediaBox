using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using Microsoft.Maps.MapControl.WPF;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// 独自の<see cref="GpsLocation"/>とMapコントロールが使用する<see cref="Location"/>の変換を行うコンバーター
	/// </summary>
	public class GpsLocationToLocationConverter : IValueConverter {
		/// <summary>
		/// <see cref="GpsLocation"/>→<see cref="Location"/>コンバート
		/// </summary>
		/// <param name="value">値(<see cref="GpsLocation"/>)</param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>変換後<see cref="Location"/></returns>
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is GpsLocation location) {
				return new Location(location.Latitude, location.Longitude);
			}
			if (value == DependencyProperty.UnsetValue) {
				return DependencyProperty.UnsetValue;
			}
			return null;
		}

		/// <summary>
		/// <see cref="Location"/>→<see cref="GpsLocation"/>コンバート
		/// </summary>
		/// <param name="value">値(<see cref="Location"/>)</param>
		/// <param name="targetTypes">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>変換後<see cref="GpsLocation"/></returns>
		public object? ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture) {
			if (value is Location location) {
				return new GpsLocation(location.Latitude, location.Longitude);
			}
			return null;
		}
	}
}
