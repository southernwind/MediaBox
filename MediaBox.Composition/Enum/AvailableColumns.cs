using System;
using System.Collections.Generic;
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
		/// ファイルパス
		/// </summary>
		FilePath,
		/// <summary>
		/// ファイルサイズ
		/// </summary>
		FileSize,
		/// <summary>
		/// 作成日時
		/// </summary>
		CreationTime,
		/// <summary>
		/// 編集日時
		/// </summary>
		ModifiedTime,
		/// <summary>
		/// 最終アクセス日時
		/// </summary>
		LastAccessTime,
		/// <summary>
		/// 解像度
		/// </summary>
		Resolution,
		/// <summary>
		/// 座標
		/// </summary>
		Location,
		/// <summary>
		/// 評価
		/// </summary>
		Rate,
		/// <summary>
		/// 不正なファイル
		/// </summary>
		IsInvalid,
		/// <summary>
		/// タグ
		/// </summary>
		Tags
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

			var dict = new Dictionary<AvailableColumns, string> {
				{ AvailableColumns.Thumbnail, "サムネイル" },
				{ AvailableColumns.FileName, "ファイル名" },
				{ AvailableColumns.FilePath, "ファイルパス" },
				{ AvailableColumns.FileSize, "ファイルサイズ" },
				{ AvailableColumns.CreationTime, "作成日時" },
				{ AvailableColumns.ModifiedTime, "編集日時" },
				{ AvailableColumns.LastAccessTime, "最終アクセス日時" },
				{ AvailableColumns.Resolution, "解像度" },
				{ AvailableColumns.Location, "座標" },
				{ AvailableColumns.Rate, "評価" },
				{ AvailableColumns.IsInvalid, "不正ファイル" },
				{ AvailableColumns.Tags, "タグ" }
			};

			if (dict.TryGetValue(ac, out var val)) {
				return val;
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}