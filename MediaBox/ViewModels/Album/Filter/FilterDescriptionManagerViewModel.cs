using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Album.Filter {
	internal class FilterDescriptionManagerViewModel : ViewModelBase {
		private readonly FilterDescriptionManager _model;
		/// <summary>
		/// フィルター条件
		/// </summary>
		public ReadOnlyReactiveCollection<IFilterItemCreator> FilterItems {
			get;
		}

		/// <summary>
		/// タグフィルター追加コマンド
		/// </summary>
		public ReactiveCommand<string> AddTagFilterCommand {
			get;
		} = new ReactiveCommand<string>();


		/// <summary>
		/// ファイルパスフィルター追加コマンド
		/// </summary>
		public ReactiveCommand<string> AddFilePathFilterCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// 評価
		/// </summary>
		public IReactiveProperty<int> Rate {
			get;
		} = new ReactivePropertySlim<int>();

		/// <summary>
		/// 評価フィルター追加コマンド
		/// </summary>
		public ReactiveCommand<int> AddRateFilterCommand {
			get;
		} = new ReactiveCommand<int>();

		/// <summary>
		/// フィルター削除コマンド
		/// </summary>
		public ReactiveCommand<IFilterItemCreator> RemoveFilterCommand {
			get;
		} = new ReactiveCommand<IFilterItemCreator>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FilterDescriptionManagerViewModel() {
			this._model = Get.Instance<FilterDescriptionManager>();
			this.FilterItems = this._model.FilterItemCreators.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			this.AddTagFilterCommand.Subscribe(this._model.AddTagFilter).AddTo(this.CompositeDisposable);
			this.AddFilePathFilterCommand.Subscribe(this._model.AddFilePathFilter).AddTo(this.CompositeDisposable);
			this.AddRateFilterCommand.Subscribe(this._model.AddRateFilter).AddTo(this.CompositeDisposable);
			this.RemoveFilterCommand.Subscribe(this._model.RemoveFilter).AddTo(this.CompositeDisposable);
		}
	}
}
