using System;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	public class ResolutionFilterItemCreator : IFilterItemCreator {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				return $"解像度が{this.Resolution}以上";
			}
		}

		/// <summary>
		/// 評価
		/// </summary>
		public ComparableSize Resolution {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public ResolutionFilterItemCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public ResolutionFilterItemCreator(ComparableSize resolution) {
			this.Resolution = resolution;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			return new FilterItem(x => x.Resolution >= this.Resolution, nameof(IMediaFileViewModel.Resolution));
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName}>";
		}
	}
}
