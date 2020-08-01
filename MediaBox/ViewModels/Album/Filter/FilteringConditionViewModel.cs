using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.ViewModels.Album.Filter.Creators;

namespace SandBeige.MediaBox.ViewModels.Album.Filter {
	/// <summary>
	/// フィルタリング条件ViewModel
	/// </summary>
	public class FilteringConditionViewModel : ViewModelBase {
		/// <summary>
		/// モデル
		/// </summary>
		public FilteringCondition Model {
			get;
		}

		/// <summary>
		/// 表示名
		/// </summary>
		public IReactiveProperty<string> DisplayName {
			get;
		}

		/// <summary>
		/// フィルター条件クリエイター
		/// </summary>
		public ReadOnlyReactiveCollection<IFilterItemCreator> FilterItems {
			get;
		}

		/// <summary>
		/// フィルター条件作成VMリスト
		/// </summary>
		public IEnumerable<IFilterCreatorViewModel> FilterCreatorViewModels {
			get;
		}

		/// <summary>
		/// 選択中フィルター条件作成VM
		/// </summary>
		public IReactiveProperty<IFilterCreatorViewModel> SelectedFilterCreatorViewModel {
			get;
		} = new ReactivePropertySlim<IFilterCreatorViewModel>();

		/// <summary>
		/// フィルター削除コマンド
		/// </summary>
		public ReactiveCommand<IFilterItemCreator> RemoveFilterCommand {
			get;
		} = new ReactiveCommand<IFilterItemCreator>();


		public FilteringConditionViewModel(FilteringCondition model) {
			this.Model = model;
			this.ModelForToString = this.Model;

			this.DisplayName = this.Model.DisplayName.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.FilterItems = this.Model.FilterItemCreators.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			this.FilterCreatorViewModels = new IFilterCreatorViewModel[] {
				new ExistsFilterCreatorViewModel(model),
				new FilePathFilterCreatorViewModel(model),
				new LocationFilterCreatorViewModel(model),
				new MediaTypeFilterCreatorViewModel(model),
				new RateFilterCreatorViewModel(model),
				new ResolutionFilterCreatorViewModel(model),
				new TagFilterCreatorViewModel(model)
			};
			this.SelectedFilterCreatorViewModel.Value = this.FilterCreatorViewModels.First();

			// 削除
			this.RemoveFilterCommand.Subscribe(this.Model.RemoveFilter).AddTo(this.CompositeDisposable);
		}
	}
}
