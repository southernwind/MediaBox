using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace SandBeige.MediaBox.Controls.Converters {
	public class XamlToUIElementConverter : IValueConverter {
		private static readonly string[] _suffix = { "", "K", "M", "G", "T" };
		private const double _unit = 1024;

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value is string xaml) {
				try {
					return (UIElement)XamlReader.Parse(xaml);
				} catch (Exception ex) {
					return new TextBlock { Text = ex.ToString() };
				}
			}
			return new TextBlock { Text = "エラー" };
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

	}
}
