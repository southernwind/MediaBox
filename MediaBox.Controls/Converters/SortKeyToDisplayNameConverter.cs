using System;
using System.Collections.Generic;
using System.Windows.Data;

using SandBeige.MediaBox.Composition.Enum;

namespace SandBeige.MediaBox.Controls.Converters {
	public class SortKeyToDisplayNameConverter : IValueConverter {
		private static readonly Dictionary<SortItemKeys, string> _names = new Dictionary<SortItemKeys, string>() {
			{SortItemKeys.FileName,"ファイル名" },
			{SortItemKeys.FilePath,"ファイルパス" },
			{SortItemKeys.CreationTime,"作成日時" },
			{SortItemKeys.ModifiedTime,"編集日時" },
			{SortItemKeys.LastAccessTime,"最終アクセス日時" },
			{SortItemKeys.FileSize,"ファイルサイズ" },
			{SortItemKeys.Location,"場所" },
			{SortItemKeys.Rate,"評価" },
			{SortItemKeys.Resolution,"解像度" },
		};

		public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value is SortItemKeys sik) {
				return _names[sik];
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

	}
}
