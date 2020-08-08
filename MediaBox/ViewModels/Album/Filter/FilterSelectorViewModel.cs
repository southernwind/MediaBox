using System.Reactive.Linq;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Views.Album.Filter;
namespace SandBeige.MediaBox.ViewModels.Album.Filter {
	/// <summary>
	/// フィルターセレクターViewModel
	/// </summary>
	public class FilterSelectorViewModel : ViewModelBase {
		private readonly IStates _states;
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
		public FilterSelectorViewModel(FilterDescriptionManager model, IDialogService dialogService, IStates states, ViewModelFactory viewModelFactory) {
			this._states = states;
			this.ModelForToString = model;
			this.FilteringConditions = model.FilteringConditions.ToReadOnlyReactiveCollection(viewModelFactory.Create);
			this.CurrentCondition = model.CurrentFilteringCondition.ToReactivePropertyAsSynchronized(
				x => x.Value,
				x => viewModelFactory.Create(x),
				x => x?.Model);

			// フィルター設定ウィンドウオープンコマンド
			this.OpenSetFilterWindowCommand.Subscribe(() => {
				dialogService.Show(nameof(SetFilterWindow), null, null);
			}).AddTo(this.CompositeDisposable);
		}

		protected override void Dispose(bool disposing) {
			this._states.Save();
			base.Dispose(disposing);
		}
	}
}
