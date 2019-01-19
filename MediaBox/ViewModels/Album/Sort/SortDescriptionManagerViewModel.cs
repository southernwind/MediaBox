
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Album.Filter {
	internal class SortDescriptionManagerViewModel : ViewModelBase {
		private readonly SortDescriptionManager _model;
		/// <summary>
		/// フィルター条件
		/// </summary>
		public ReadOnlyReactiveCollection<SortItem> SortItems {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SortDescriptionManagerViewModel() {
			this._model = Get.Instance<SortDescriptionManager>();
			this.SortItems = this._model.SortItems.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
		}
	}
}
