using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

using SandBeige.MediaBox.Controls.Converters;

namespace SandBeige.MediaBox.Plugins.Viewer.TileStyle {
	public static class ZoomLevel {
		public static int DefaultLevel = 5;
		public static int MaxLevel = 8;
		public static int MinLevel = 1;
		public static readonly Dictionary<int, int> SizeList = new Dictionary<int, int> {
			{1,50 },
			{2,70 },
			{3,90 },
			{4,120 },
			{5,150 },
			{6,180 },
			{7,210 },
			{8,250 },
		};

		public static int GetSize(object level) {
			var val = level as int? ?? DefaultLevel;
			if (MaxLevel < val) {
				return SizeList[MaxLevel];
			}
			if (MinLevel > val) {
				return SizeList[MinLevel];
			}
			return SizeList[val];
		}
	}

	/// <summary>
	/// ズームレベルを画像の一辺の長さに変換するコンバーター
	/// </summary>
	public class ZoomLevelToImageSideWidthLengthConverter : IValueConverter {
		/// <summary>
		/// コンバート
		/// </summary>
		/// <param name="value">ズームレベル</param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>画像の一辺の長さ</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var converter = new ZoomLevelToImageSideLengthConverter();
			var result = converter.Convert(value, targetType, parameter, culture);
			if (result is int i) {
				return i * 3;
			}

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
