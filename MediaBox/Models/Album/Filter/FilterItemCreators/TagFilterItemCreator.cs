using System;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// タグフィルタークリエイター
	/// </summary>
	public class TagFilterItemCreator : IFilterItemCreator {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				return $"{this.TagName}をタグに含む";
			}
		}

		/// <summary>
		/// タグ名
		/// </summary>
		public string TagName {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public TagFilterItemCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public TagFilterItemCreator(string tagName) {
			this.TagName = tagName;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			return new FilterItem(x => x.Tags.Contains(this.TagName), nameof(IMediaFileViewModel.Tags));
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName}>";
		}
	}
}
