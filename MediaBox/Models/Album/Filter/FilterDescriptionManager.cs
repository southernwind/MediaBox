using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace SandBeige.MediaBox.Models.Album.Filter {
	internal class FilterDescriptionManager : ModelBase {
		private readonly ReactiveCollection<FilterItem> _filterItems;

		/// <summary>
		/// フィルター条件
		/// </summary>
		public ReadOnlyReactiveCollection<FilterItem> FilterItems {
			get;
		}


		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FilterDescriptionManager() {
			this._filterItems = new ReactiveCollection<FilterItem>();
			this.FilterItems = this._filterItems.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// フィルター追加
		/// </summary>
		/// <param name="tagName"></param>
		public void AddFilterTag(string tagName) {
			this._filterItems.Add(
				new FilterItem(
					x => x.Tags.Contains(tagName),
					$"{tagName}をタグに含む"
				)
			);
		}

		/// <summary>
		/// フィルター削除
		/// </summary>
		/// <param name="item"></param>
		public void RemoveFilter(FilterItem item) {
			this._filterItems.Remove(item);
		}
	}
}
