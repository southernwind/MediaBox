
using System;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Models.Album.Filter;

namespace SandBeige.MediaBox.ViewModels.Album.Filter {
	/// <summary>
	/// フィルター設定ウィンドウViewModel
	/// </summary>
	public class SetFilterWindowViewModel : DialogViewModelBase {
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
		/// フィルタリング条件追加コマンド
		/// </summary>
		public ReactiveCommand AddFilteringConditionCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// フィルタリング条件削除コマンド
		/// </summary>
		public ReactiveCommand<FilteringConditionViewModel> RemoveFilteringConditionCommand {
			get;
		} = new ReactiveCommand<FilteringConditionViewModel>();

		/// <summary>
		/// フィルター設定ウィンドウオープン
		/// </summary>
		public ReactiveCommand OpenSetFilterWindowCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// ウィンドウタイトル
		/// </summary>
		public override string Title {
			get {
				return "フィルター設定";
			}
			set {
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SetFilterWindowViewModel(FilterDescriptionManager model, IStates states, ViewModelFactory viewModelFactory) {
			this._states = states;
			model.Name.Value = "set";
			this.ModelForToString = model;
			this.FilteringConditions = model.FilteringConditions.ToReadOnlyReactiveCollection(viewModelFactory.Create);
			this.CurrentCondition = model.CurrentFilteringCondition.ToReactivePropertyAsSynchronized(
				x => x.Value,
				x => viewModelFactory.Create(x),
				x => x?.Model);

			this.AddFilteringConditionCommand.Subscribe(model.AddCondition);

			this.RemoveFilteringConditionCommand.Where(x => x != null).Subscribe(x => {
				model.RemoveCondition(x.Model);
			});
		}

		protected override void Dispose(bool disposing) {
			this._states.Save();
			base.Dispose(disposing);
		}
	}
}
