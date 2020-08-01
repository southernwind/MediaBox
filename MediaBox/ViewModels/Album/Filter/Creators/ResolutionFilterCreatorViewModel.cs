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
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;

namespace SandBeige.MediaBox.ViewModels.Album.Filter.Creators {
	/// <summary>
	/// 解像度フィルター作成ViewModel
	/// </summary>
	public class ResolutionFilterCreatorViewModel : ViewModelBase, IFilterCreatorViewModel {
		/// <summary>
		/// 表示名
		/// </summary>
		public string Title {
			get {
				return "解像度フィルター";
			}
		}

		/// <summary>
		/// アイコン
		/// </summary>
		public PackIconBase Icon {
			get {
				return new PackIconModern { Kind = PackIconModernKind.DimensionArrowLineWidth };
			}
		}

		/// <summary>
		/// 解像度フィルター追加コマンド
		/// </summary>
		public ReactiveCommand AddResolutionFilterCommand {
			get;
		}

		/// <summary>
		/// 解像度幅
		/// </summary>
		[Range(0d, int.MaxValue)]
		public ReactiveProperty<string> ResolutionWidthText {
			get;
		}

		/// <summary>
		/// 解像度高さ
		/// </summary>
		[Range(0d, int.MaxValue)]
		public ReactiveProperty<string> ResolutionHeightText {
			get;
		}

		/// <summary>
		/// 検索タイプを選択
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

		public ResolutionFilterCreatorViewModel(FilteringCondition model) {
			this.ModelForToString = model;

			this.ResolutionWidthText = new ReactiveProperty<string>().SetValidateAttribute(() => this.ResolutionWidthText);
			this.ResolutionHeightText = new ReactiveProperty<string>().SetValidateAttribute(() => this.ResolutionHeightText);
			this.SearchType.Value = this.SearchTypeList.First(x => x.Value == SearchTypeComparison.GreaterThanOrEqual);

			this.AddResolutionFilterCommand =
				new[] {
					new[] {
							this.ResolutionWidthText.Select(string.IsNullOrEmpty),
							this.ResolutionHeightText.Select(string.IsNullOrEmpty)
						}
						.CombineLatest(x => x.All(b => b)),
					this.ResolutionWidthText.ObserveHasErrors,
					this.ResolutionHeightText.ObserveHasErrors
				}.CombineLatestValuesAreAllFalse()
				.ToReactiveCommand();

			this.AddResolutionFilterCommand
				.Subscribe(_ => {
					int? width = null;
					int? height = null;
					if (int.TryParse(this.ResolutionWidthText.Value, out var w)) {
						width = w;
					}
					if (int.TryParse(this.ResolutionHeightText.Value, out var h)) {
						height = h;
					}
					model.AddResolutionFilter(width, height, this.SearchType.Value.Value);

					this.ResolutionWidthText.Value = null;
					this.ResolutionHeightText.Value = null;
				})
				.AddTo(this.CompositeDisposable);
		}
	}
}
