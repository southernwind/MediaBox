using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Album.Filter {
	internal class FilterDescriptionManagerViewModel : ViewModelBase {
		private readonly FilterDescriptionManager _model;
		/// <summary>
		/// フィルター条件
		/// </summary>
		public ReadOnlyReactiveCollection<FilterItem> FilterItems {
			get;
		}

		/// <summary>
		/// タグフィルター追加コマンド
		/// </summary>
		public ReactiveCommand<string> AddTagFilterCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// フィルター削除コマンド
		/// </summary>
		public ReactiveCommand<FilterItem> RemoveFilterCommand {
			get;
		} = new ReactiveCommand<FilterItem>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FilterDescriptionManagerViewModel() {
			this._model = Get.Instance<FilterDescriptionManager>();
			this.FilterItems = this._model.FilterItems.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			this.AddTagFilterCommand.Subscribe(this._model.AddTagFilter).AddTo(this.CompositeDisposable);
			this.RemoveFilterCommand.Subscribe(this._model.RemoveFilter).AddTo(this.CompositeDisposable);
		}
	}
}
