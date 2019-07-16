using System;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// 解像度フィルタークリエイター
	/// </summary>
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
		/// 解像度
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
		/// <param name="resolution">解像度</param>
		public ResolutionFilterItemCreator(ComparableSize resolution) {
			this.Resolution = resolution;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			return new FilterItem(x => x.Width * x.Height >= this.Resolution.Area);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName}>";
		}
	}
}
