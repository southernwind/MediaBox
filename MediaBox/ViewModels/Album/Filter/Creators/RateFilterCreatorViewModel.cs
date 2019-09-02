using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;

namespace SandBeige.MediaBox.ViewModels.Album.Filter.Creators {
	/// <summary>
	/// 評価フィルター作成ViewModel
	/// </summary>
	internal class RateFilterCreatorViewModel : ViewModelBase, IFilterCreatorViewModel {
		/// <summary>
		/// 表示名
		/// </summary>
		public string Title {
			get {
				return "評価フィルター";
			}
		}

		/// <summary>
		/// 評価
		/// </summary>
		public IReactiveProperty<int?> Rate {
			get;
		} = new ReactivePropertySlim<int?>();


		/// <summary>
		/// 検索条件として指定のタグを含むものを検索するか、含まないものを検索するかを選択する。
		/// </summary>
		public IReactiveProperty<BindingItem<SearchTypeComparison>> SearchType {
			get;
		} = new ReactivePropertySlim<BindingItem<SearchTypeComparison>>();

		/// <summary>
		/// 含む/含まないの選択候補
		/// </summary>
		public IEnumerable<BindingItem<SearchTypeComparison>> SearchTypeList {
			get;
		} = new[] {
			new BindingItem<SearchTypeComparison>("を超える",SearchTypeComparison.GreaterThan),
			new BindingItem<SearchTypeComparison>("以上",SearchTypeComparison.GreaterThanOrEqual),
			new BindingItem<SearchTypeComparison>("と等しい",SearchTypeComparison.Equal),
			new BindingItem<SearchTypeComparison>("以下",SearchTypeComparison.LessThanOrEqual),
			new BindingItem<SearchTypeComparison>("未満",SearchTypeComparison.LessThan)
		};

		/// <summary>
		/// 評価フィルター追加コマンド
		/// </summary>
		public ReactiveCommand AddRateFilterCommand {
			get;
		}

		public RateFilterCreatorViewModel(FilteringCondition model) {
			this.ModelForToString = model;

			this.SearchType.Value = this.SearchTypeList.First(x => x.Value == SearchTypeComparison.GreaterThanOrEqual);
			this.AddRateFilterCommand = this.Rate.Select(x => x.HasValue).ToReactiveCommand().AddTo(this.CompositeDisposable);
			this.AddRateFilterCommand
				.Subscribe(_ => {
					if (this.Rate.Value is { } r) {
						model.AddRateFilter(r, this.SearchType.Value.Value);
					}

					this.Rate.Value = null;
				})
				.AddTo(this.CompositeDisposable);

		}
	}
}
