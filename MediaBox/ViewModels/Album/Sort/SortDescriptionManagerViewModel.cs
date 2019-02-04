using System.ComponentModel;
using System.Windows.Data;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Album.Filter {
	/// <summary>
	/// <see cref="SortItem.UpdateTime"/>でソートされた<see cref="CollectionView"/>を持つ<see cref="SortItems"/>を公開する。
	/// </summary>
	internal class SortDescriptionManagerViewModel : ViewModelBase {
		/// <summary>
		/// モデル
		/// </summary>
		private readonly SortDescriptionManager _model;
		/// <summary>
		/// ソート条件候補
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

			if (collectionView is ICollectionViewLiveShaping cvls && cvls.CanChangeLiveSorting) {
				cvls.LiveSortingProperties.Clear();
				cvls.LiveSortingProperties.AddRange(nameof(SortItem.Enabled), nameof(SortItem.UpdateTime));
				cvls.IsLiveSorting = true;
			}
		}
	}
}
