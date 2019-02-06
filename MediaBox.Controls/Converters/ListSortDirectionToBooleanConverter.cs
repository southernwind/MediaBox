using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// 昇順/降順⇔booleanコンバータ
	/// </summary>
	/// <remarks>
	/// ConvertWhenTrueに設定した方をTrueとして、もう一方をFalseとする
	/// </remarks>
	public class ListSortDirectionToBooleanConverter : IValueConverter {
		/// <summary>
		/// Trueにするほう
		/// </summary>
		public ListSortDirection ConvertWhenTrue {
			private get;
			set;
		} = ListSortDirection.Ascending;

		/// <summary>
		/// コンバート
		/// </summary>
		/// <param name="value">変換前値(<see cref="ListSortDirection"/>)</param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>変換後値(<see cref="bool"/>)</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is ListSortDirection listSortDirection) {
				return listSortDirection == this.ConvertWhenTrue;
			}
			return false;
		}

		/// <summary>
		/// コンバートバック
		/// </summary>
		/// <param name="value">変換前値(<see cref="bool"/>)</param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>変換後値(<see cref="ListSortDirection"/>)</returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			var b = false;
			if (value is bool bb) {
				b = bb;
			}
			if (this.ConvertWhenTrue != ListSortDirection.Ascending) {
				b = !b;
			}
			return b ? ListSortDirection.Ascending : ListSortDirection.Descending;
		}
	}
}
