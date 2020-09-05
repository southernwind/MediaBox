using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;

using MahApps.Metro.IconPacks;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.ViewModels.Album.Filter.Creators {
	/// <summary>
	/// 評価フィルター作成ViewModel
	/// </summary>
	public class RateFilterCreatorViewModel : ViewModelBase, IFilterCreatorViewModel {
		/// <summary>
		/// 表示名
		/// </summary>
		public string Title {
			get {
				return "評価フィルター";
			}
		}

		/// <summary>
		/// アイコン
		/// </summary>
		public PackIconBase Icon {
			get {
				return new PackIconMaterial { Kind = PackIconMaterialKind.Star };
			}
		}

		/// <summary>
		/// 評価 チェック用テキスト
		/// </summary>
		[Range(0, 5)]
		public ReactiveProperty<string?> RateText {
			get;
		}

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

		public RateFilterCreatorViewModel(IFilteringCondition model) {
			this.ModelForToString = model;

			this.RateText = new ReactiveProperty<string?>().SetValidateAttribute(() => this.RateText);
			this.SearchType.Value = this.SearchTypeList.First(x => x.Value == SearchTypeComparison.GreaterThanOrEqual);
			this.AddRateFilterCommand = new[] {
					this.RateText.Select(string.IsNullOrEmpty),
					this.RateText.ObserveHasErrors
				}.CombineLatestValuesAreAllFalse()
				.ToReactiveCommand()
				.AddTo(this.CompositeDisposable);
			this.AddRateFilterCommand
				.Subscribe(_ => {
					if (int.TryParse(this.RateText.Value, out var r)) {
						model.AddRateFilter(r, this.SearchType.Value.Value);
					}

					this.RateText.Value = null;
				})
				.AddTo(this.CompositeDisposable);

		}
	}
}
