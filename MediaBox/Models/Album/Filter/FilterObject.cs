
using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;

namespace SandBeige.MediaBox.Models.Album.Filter {
	/// <summary>
	/// フィルター設定復元用オブジェクト
	/// </summary>
	public class FilterObject : IFilterObject {
		/// <summary>
		/// 表示名
		/// </summary>
		public IReactiveProperty<string> DisplayName {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// フィルター条件オブジェクト
		/// </summary>
		public ReactiveCollection<IFilterItemObject> FilterItemObjects {
			get;
			set;
		} = new ReactiveCollection<IFilterItemObject>();
	}
}
