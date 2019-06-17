using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.Converters {
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
	/// ズームレベルをVirtualizeWrapPanelのItemSizeに変換するコンバーター
	/// </summary>
	public class ZoomLevelToItemSizeConverter : IValueConverter {
		/// <summary>
		/// コンバート
		/// </summary>
		/// <param name="value">ズームレベル</param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>ItemSizeを表す文字列</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var size = ZoomLevel.GetSize(value);
			return $"{size},{size}";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}

	/// <summary>
	/// ズームレベルを画像の一辺の長さに変換するコンバーター
	/// </summary>
	public class ZoomLevelToImageSideLengthConverter : IValueConverter {
		/// <summary>
		/// コンバート
		/// </summary>
		/// <param name="value">ズームレベル</param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>画像の一辺の長さ</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var size = ZoomLevel.GetSize(value);
			return size - 10;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
