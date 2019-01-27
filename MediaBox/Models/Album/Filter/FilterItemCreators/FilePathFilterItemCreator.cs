using System;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	public class FilePathFilterItemCreator : IFilterItemCreator {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				return $"{this.Text}をタグに含む";
			}
		}

		/// <summary>
		/// パスに含まれる文字列
		/// </summary>
		public string Text {
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
		public FilePathFilterItemCreator(string text) {
			this.Text = text;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			return new FilterItem(x => x.FilePath.Contains(this.Text));
		}
		public override string ToString() {
			return $"<[{base.ToString()}] {this.Text}>";
		}
	}
}
