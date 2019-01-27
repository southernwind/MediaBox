
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Data;

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
			this.ModelForToString = this._model;
			this.SortItems = this._model.SortItems.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			var collectionView = CollectionViewSource.GetDefaultView(this.SortItems);

			collectionView.SortDescriptions.Clear();
			collectionView.SortDescriptions.Add(new SortDescription(nameof(SortItem.Enabled), ListSortDirection.Descending));
			collectionView.SortDescriptions.Add(new SortDescription(nameof(SortItem.UpdateTime), ListSortDirection.Ascending));

			this.SortItems.ObserveElementProperty(x => x.Enabled).ToUnit()
				.Merge(this.SortItems.ObserveElementProperty(x => x.Direction).Where(x => x.Instance.Enabled).ToUnit())
				.Subscribe(x => collectionView.Refresh());
		}
	}
}
