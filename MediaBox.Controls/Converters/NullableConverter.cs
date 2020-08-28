using System;
using System.Globalization;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// 空文字をnullに変換するコンバーター
	/// </summary>
	public class NullableConverter : IValueConverter {
		/// <summary>
		/// コンバーター(引数を素通しするだけ)
		/// </summary>
		/// <param name="value">そのまま返却する値</param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>引数で渡された値</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}

		/// <summary>
		/// コンバーターバック(こっちが本質)
		/// </summary>
		/// <remarks>
		/// 変換後型がNullableであり、valueの値がstring.Emptyならば、null
		/// それ以外の場合はそのまま引数の値が返る。
		/// あとはデフォルトコンバーターにおまかせ。
		/// </remarks>
		/// <param name="value">変換前値</param>
		/// <param name="targetType">変換後型</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>変換後値</returns>
		public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
				value is string text &&
				string.IsNullOrEmpty(text)) {
				return null;
			}
			return value;
		}
	}
}
