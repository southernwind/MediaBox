using System.Reactive.Linq;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Views.Album.Filter;

namespace SandBeige.MediaBox.ViewModels.Album.Filter {
	/// <summary>
	/// フィルターセレクターViewModel
	/// </summary>
	internal class FilterSelectorViewModel : ViewModelBase {

		/// <summary>
		/// カレント条件
		/// </summary>
		public IReactiveProperty<FilteringConditionViewModel> CurrentCondition {
			get;
		}

		/// <summary>
		/// フィルタリング条件
		/// </summary>
		public ReadOnlyReactiveCollection<FilteringConditionViewModel> FilteringConditions {
			get;
		}

		/// <summary>
		/// フィルター設定ウィンドウオープン
		/// </summary>
		public ReactiveCommand OpenSetFilterWindowCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FilterSelectorViewModel(FilterDescriptionManager model, IDialogService dialogService) {
			this.ModelForToString = model;
			this.FilteringConditions = model.FilteringConditions.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create);
			this.CurrentCondition = model.CurrentFilteringCondition.ToReactivePropertyAsSynchronized(
				x => x.Value,
				x => this.ViewModelFactory.Create(x),
				x => x?.Model);

			// フィルター設定ウィンドウオープンコマンド
			this.OpenSetFilterWindowCommand.Subscribe(() => {
				dialogService.Show(nameof(SetFilterWindow), null, null);
			}).AddTo(this.CompositeDisposable);
		}

		protected override void Dispose(bool disposing) {
			this.States.Save();
			base.Dispose(disposing);
		}
	}
}
