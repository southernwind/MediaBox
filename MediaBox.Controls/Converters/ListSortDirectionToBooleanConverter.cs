using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// 昇順/降順⇔booleanコンバータ
	/// ConvertWhenTrueに設定した方をTrueとして、もう一方をFalseとする
	/// </summary>
	public class ListSortDirectionToBooleanConverter : IValueConverter {
		/// <summary>
		/// Trueにするほう
		/// </summary>
		public ListSortDirection ConvertWhenTrue {
			private get;
			set;
		} = ListSortDirection.Ascending;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is ListSortDirection listSortDirection) {
				return listSortDirection == this.ConvertWhenTrue;
			}
			return false;
		}

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
