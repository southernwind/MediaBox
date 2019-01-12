using System;
using System.Windows;
using System.Windows.Data;

namespace SandBeige.MediaBox.Composition.Enum {
	/// <summary>
	/// 表示モード
	/// </summary>
	public enum DisplayMode {
		/// <summary>
		/// ライブラリ表示
		/// </summary>
		Library,
		/// <summary>
		/// 詳細表示
		/// </summary>
		Detail,
		/// <summary>
		/// マップ表示
		/// </summary>
		Map
	}

	public class DisplayModeToStringConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value != null && value is DisplayMode) {
				switch ((DisplayMode)value) {
					case DisplayMode.Library:
						return "ライブラリ表示";

					case DisplayMode.Detail:
						return "詳細表示";

					case DisplayMode.Map:
						return "マップ表示";
					default:
						break;
				}
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}