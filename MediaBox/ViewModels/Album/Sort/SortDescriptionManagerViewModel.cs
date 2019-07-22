using System.ComponentModel;
using System.Windows.Data;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.ViewModels.Album.Sort {

	/// <summary>
	/// <see cref="SortItem.UpdateTime"/>でソートされた<see cref="CollectionView"/>を持つ<see cref="SortItems"/>を公開する。
	/// </summary>
	internal class SortDescriptionManagerViewModel : ViewModelBase {
		/// <summary>
		/// ソート条件候補
		/// </summary>
		public ReadOnlyReactiveCollection<ISortItem> SortItems {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SortDescriptionManagerViewModel(SortDescriptionManager model) {
			this.ModelForToString = model;
			this.SortItems = model.SortItems.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			var collectionView = CollectionViewSource.GetDefaultView(this.SortItems);

			collectionView.SortDescriptions.Clear();
			collectionView.SortDescriptions.Add(new SortDescription(nameof(ISortItem.Enabled), ListSortDirection.Descending));
			collectionView.SortDescriptions.Add(new SortDescription(nameof(ISortItem.UpdateTime), ListSortDirection.Ascending));

			if (collectionView is ICollectionViewLiveShaping cvls && cvls.CanChangeLiveSorting) {
				cvls.LiveSortingProperties.Clear();
				cvls.LiveSortingProperties.AddRange(nameof(ISortItem.Enabled), nameof(ISortItem.UpdateTime));
				cvls.IsLiveSorting = true;
			}
		}
	}
}
