using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// すべての値が<see cref="DependencyProperty.UnsetValue"/>でない場合にtrueを返すコンバーター
	/// </summary>
	public class AllValuesAreSetConverter : IMultiValueConverter {
		/// <summary>
		/// コンバート
		/// </summary>
		/// <param name="values">値リスト</param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>true : すべての値がセットされている false : 一つ以上値がセットされていないものがある</returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			return values.All(x => x != DependencyProperty.UnsetValue && x != null);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
