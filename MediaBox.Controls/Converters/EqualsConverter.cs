using System;
using System.Globalization;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// 等価コンバータ
	/// </summary>
	public class EqualsConverter : IMultiValueConverter {
		/// <summary>
		/// コンバーター
		/// </summary>
		/// <remarks>
		/// 引数のvalues[0]とvalues[1]を比較してその結果を返す
		/// </remarks>
		/// <param name="values">比較する値 [0],[1]</param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>比較結果</returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			return values[0]?.Equals(values[1]) ?? values[1] == null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
