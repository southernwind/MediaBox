using System;
using System.Windows;
using System.Windows.Data;

namespace SandBeige.MediaBox.Composition.Enum {
	/// <summary>
	/// 表示列
	/// </summary>
	public enum AvailableColumns {
		/// <summary>
		/// サムネイル
		/// </summary>
		Thumbnail,
		/// <summary>
		/// ファイル名
		/// </summary>
		FileName,
		/// <summary>
		/// 編集日時
		/// </summary>
		ModifiedTime,
	}

	/// <summary>
	/// AvailableColumns→stringコンバーター
	/// </summary>
	public class AvailableColumnsToStringConverter : IValueConverter {
		/// <summary>
		/// 変換
		/// </summary>
		/// <param name="value"><see cref="AvailableColumns"/></param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns><see cref="DisplayMode"/>をstringに変換したもの</returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (!(value is AvailableColumns ac)) {
				return DependencyProperty.UnsetValue;
			}

			switch (ac) {
				case AvailableColumns.Thumbnail:
					return "サムネイル";

				case AvailableColumns.FileName:
					return "ファイル名";

				case AvailableColumns.ModifiedTime:
					return "編集日時";
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}