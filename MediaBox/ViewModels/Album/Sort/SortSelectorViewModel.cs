using System;
using System.ComponentModel;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Views.Album.Sort;

namespace SandBeige.MediaBox.ViewModels.Album.Sort {

	/// <summary>
	/// ソート設定ViewModel
	/// </summary>
	public class SortSelectorViewModel : ViewModelBase {

		/// <summary>
		/// カレントソート条件
		/// </summary>
		public IReactiveProperty<SortConditionViewModel> CurrentSortCondition {
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
		/// フィルター設定ウィンドウオープン
		/// </summary>
		public ReactiveCommand OpenSetSortWindowCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// フィルター設定ウィンドウオープン
		/// </summary>
		public ReactiveCommand<SortConditionViewModel> SetCurrentSortConditionCommand {
			get;
		} = new ReactiveCommand<SortConditionViewModel>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SortSelectorViewModel(ISortDescriptionManager model, IDialogService dialogService, ViewModelFactory viewModelFactory) {
			this.ModelForToString = model;
			this.CurrentSortCondition =
				model
					.CurrentSortCondition
					.ToReactivePropertyAsSynchronized(
						x => x.Value,
						viewModelFactory.Create,
						x => x.Model)
					.AddTo(this.CompositeDisposable);
			this.SortConditions = model.SortConditions.ToReadOnlyReactiveCollection(viewModelFactory.Create).AddTo(this.CompositeDisposable);
			this.Direction = model.Direction.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			// ソート設定ウィンドウオープンコマンド
			this.OpenSetSortWindowCommand.Subscribe(() => {
				dialogService.Show(nameof(SetSortWindow), null, null);
			}).AddTo(this.CompositeDisposable);

			this.SetCurrentSortConditionCommand.Subscribe(x => {
				this.CurrentSortCondition.Value = x;
			});
		}
	}
}
