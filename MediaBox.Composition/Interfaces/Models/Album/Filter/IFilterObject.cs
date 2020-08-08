
using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter {
	public interface IFilterObject {
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
		public ReactiveCollection<IFilterItemCreator> FilterItemCreators {
			get;
			set;
		}
	}
}
