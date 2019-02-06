using System;
using System.Globalization;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// 配列Indexを選択箇所に変換するコンバーター
	/// </summary>
	/// <remarks>
	/// 選択中アイテムなし→"未選択"
	/// c[0]を選択中→"1"
	/// c[15]を選択中→"16"
	/// </remarks>
	public class IndexToPositionConverter : IValueConverter {
		/// <summary>
		/// コンバート
		/// </summary>
		/// <param name="value">インデックス</param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>選択箇所を示す文字列</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is int i) {
				if (i < 0) {
					return "未選択";
				}
				return (i + 1).ToString();
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
