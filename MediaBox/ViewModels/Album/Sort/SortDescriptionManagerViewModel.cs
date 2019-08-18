using System;
using System.ComponentModel;

using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.ViewModels.Album.Sort {

	/// <summary>
	/// ソート設定ViewModel
	/// </summary>
	internal class SortDescriptionManagerViewModel : ViewModelBase {

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
		/// フィルタリング条件追加コマンド
		/// </summary>
		public ReactiveCommand AddSortConditionCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// フィルタリング条件削除コマンド
		/// </summary>
		public ReactiveCommand<SortConditionViewModel> RemoveSortConditionCommand {
			get;
		} = new ReactiveCommand<SortConditionViewModel>();

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
		public SortDescriptionManagerViewModel(SortDescriptionManager model) {
			this.ModelForToString = model;
			this.CurrentSortCondition =
				model
					.CurrentSortCondition
					.ToReactivePropertyAsSynchronized(
						x => x.Value,
						this.ViewModelFactory.Create,
						x => x?.Model)
					.AddTo(this.CompositeDisposable);
			this.SortConditions = model.SortConditions.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create).AddTo(this.CompositeDisposable);
			this.Direction = model.Direction.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.AddSortConditionCommand.Subscribe(model.AddCondition);
			this.RemoveSortConditionCommand.Subscribe(x => model.RemoveCondition(x.Model));

			// ソート設定ウィンドウオープンコマンド
			this.OpenSetSortWindowCommand.Subscribe(() => {
				var vm = new SortDescriptionManagerViewModel(new SortDescriptionManager("set"));
				var message = new TransitionMessage(typeof(Views.Album.Sort.SetSortWindow), vm, TransitionMode.Normal);
				this.Messenger.Raise(message);
			});

			this.SetCurrentSortConditionCommand.Subscribe(x => {
				this.CurrentSortCondition.Value = x;
			});
		}
	}
}
