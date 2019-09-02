using System;

using SandBeige.MediaBox.Composition.Enum;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// ファイルパスフィルタークリエイター
	/// </summary>
	public class FilePathFilterItemCreator : IFilterItemCreator {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				return $"{this.Text}をファイルパスに{(this.SearchType == SearchType.Include ? "含む" : "含まない")}";
			}
		}

		/// <summary>
		/// パスに含まれる文字列
		/// </summary>
		public string Text {
			get;
			set;
		}

		/// <summary>
		/// 検索タイプ
		/// </summary>
		public SearchType SearchType {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public FilePathFilterItemCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="text">パスに含まれる文字列</param>
		/// <param name="searchType">検索タイプ</param>
		public FilePathFilterItemCreator(string text, SearchType searchType) {
			this.Text = text;
			this.SearchType = searchType;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			return new FilterItem(
				x => x.FilePath.Contains(this.Text) == (this.SearchType == SearchType.Include),
				x => x.FilePath.Contains(this.Text) == (this.SearchType == SearchType.Include));
		}
		public override string ToString() {
			return $"<[{base.ToString()}] {this.Text}>";
		}
	}
}
