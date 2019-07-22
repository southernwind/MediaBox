
using System;
using System.Reactive.Linq;

using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Album.Filter {
	/// <summary>
	/// フィルターマネージャーViewModel
	/// </summary>
	internal class FilterDescriptionManagerViewModel : ViewModelBase {

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
		/// コンストラクタ
		/// </summary>
		public FilterDescriptionManagerViewModel(FilterDescriptionManager model) {
			this.ModelForToString = model;
			this.FilteringConditions = model.FilteringConditions.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create);
			this.CurrentCondition = model.CurrentFilteringCondition.ToReactivePropertyAsSynchronized(
				x => x.Value,
				x => this.ViewModelFactory.Create(x),
				x => x?.Model);

			this.AddFilteringConditionCommand.Subscribe(model.AddCondition);

			this.RemoveFilteringConditionCommand.Where(x => x != null).Subscribe(x => {
				model.RemoveCondition(x.Model);
			});

			// フィルター設定ウィンドウオープンコマンド
			this.OpenSetFilterWindowCommand.Subscribe(() => {
				var vm = Get.Instance<FilterDescriptionManagerViewModel>(Get.Instance<FilterDescriptionManager>("set"));
				var message = new TransitionMessage(typeof(Views.Album.Filter.SetFilterWindow), vm, TransitionMode.Normal);
				this.Messenger.Raise(message);
			});
		}

		protected override void Dispose(bool disposing) {
			this.States.Save();
			base.Dispose(disposing);
		}
	}
}
