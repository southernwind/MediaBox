using System;
using System.ComponentModel;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;

namespace SandBeige.MediaBox.ViewModels.Album.Sort {
	/// <summary>
	/// ソート設定ウィンドウViewModel
	/// </summary>
	public class SetSortWindowViewModel : DialogViewModelBase {

		/// <summary>
		/// カレントソート条件
		/// </summary>
		public IReactiveProperty<SortConditionViewModel?> CurrentSortCondition {
			get;
		}

		/// <summary>
		/// ソート条件リスト
		/// </summary>
		public ReadOnlyReactiveCollection<SortConditionViewModel> SortConditions {
			get;
		}

		/// <summary>
		/// ソート方向
		/// </summary>
		public IReactiveProperty<ListSortDirection> Direction {
			get;
		}

		/// <summary>
		/// ソート条件追加コマンド
		/// </summary>
		public ReactiveCommand AddSortConditionCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// ソート条件削除コマンド
		/// </summary>
		public ReactiveCommand<SortConditionViewModel> RemoveSortConditionCommand {
			get;
		} = new ReactiveCommand<SortConditionViewModel>();

		/// <summary>
		/// ソート条件設定コマンド
		/// </summary>
		public ReactiveCommand<SortConditionViewModel> SetCurrentSortConditionCommand {
			get;
		} = new ReactiveCommand<SortConditionViewModel>();

		/// <summary>
		/// ウィンドウタイトル
		/// </summary>
		public override string? Title {
			get {
				return "ソート設定";
			}
			set {
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SetSortWindowViewModel(ISortDescriptionManager model, ViewModelFactory viewModelFactory) {
			model.Name.Value = "set";
			this.ModelForToString = model;
			this.CurrentSortCondition =
				model
					.CurrentSortCondition
					.ToReactivePropertyAsSynchronized<IReactiveProperty<ISortCondition?>, ISortCondition?, SortConditionViewModel?>(
						x => x.Value,
						x => x == null ? null : viewModelFactory.Create(x),
						x => x?.Model)
					.AddTo(this.CompositeDisposable);
			this.SortConditions = model.SortConditions.ToReadOnlyReactiveCollection(viewModelFactory.Create).AddTo(this.CompositeDisposable);
			this.Direction = model.Direction.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.AddSortConditionCommand.Subscribe(model.AddCondition);
			this.RemoveSortConditionCommand.Subscribe(x => model.RemoveCondition(x.Model));

			this.SetCurrentSortConditionCommand.Subscribe(x => {
				this.CurrentSortCondition.Value = x;
			});
		}
	}
}
