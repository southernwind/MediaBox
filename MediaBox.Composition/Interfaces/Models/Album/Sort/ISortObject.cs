
using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort {
	public interface ISortObject {

		/// <summary>
		/// 表示名
		/// </summary>
		public IReactiveProperty<string> DisplayName {
			get;
			set;
		}

		/// <summary>
		/// フィルター条件クリエイター
		/// </summary>
		public ReactiveCollection<ISortItemCreator> SortItemCreators {
			get;
			set;
		}
	}
}
