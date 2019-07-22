using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;

namespace SandBeige.MediaBox.ViewModels.Album.Filter {
	/// <summary>
	/// フィルタリング条件ViewModel
	/// </summary>
	internal class FilteringConditionViewModel : ViewModelBase {
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
		public IReactiveProperty<int?> Rate {
			get;
		} = new ReactivePropertySlim<int?>();

		/// <summary>
		/// 評価フィルター追加コマンド
		/// </summary>
		public ReactiveCommand AddRateFilterCommand {
			get;
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
		public IReactiveProperty<int?> ResolutionWidth {
			get;
		} = new ReactivePropertySlim<int?>();

		/// <summary>
		/// 解像度高さ
		/// </summary>
		public IReactiveProperty<int?> ResolutionHeight {
			get;
		} = new ReactivePropertySlim<int?>();


		/// <summary>
		/// メディアタイプフィルター追加コマンド
		/// </summary>
		public ReactiveCommand AddMediaTypeFilterCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// メディアタイプ
		/// </summary>
		public IReactiveProperty<BindingItem<bool>> MediaType {
			get;
		} = new ReactivePropertySlim<BindingItem<bool>>();

		/// <summary>
		/// メディアタイプ候補
		/// </summary>
		public IEnumerable<BindingItem<bool>> MediaTypeList {
			get;
		} = new[] {
			new BindingItem<bool>("画像",false),
			new BindingItem<bool>("動画",true)
		};

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
			// タグ
			this.AddTagFilterCommand.Subscribe(this.Model.AddTagFilter).AddTo(this.CompositeDisposable);
			// ファイルパス
			this.AddFilePathFilterCommand.Subscribe(this.Model.AddFilePathFilter).AddTo(this.CompositeDisposable);
			// 評価
			this.AddRateFilterCommand = this.Rate.Select(x => x.HasValue).ToReactiveCommand();
			this.AddRateFilterCommand
				.Subscribe(_ => {
					if (this.Rate.Value is { } r) {
						this.Model.AddRateFilter(r);
					}
					this.Rate.Value = null;
				})
				.AddTo(this.CompositeDisposable);
			// 解像度
			this.AddResolutionFilterCommand =
				this.ResolutionWidth
					.CombineLatest(this.ResolutionHeight, (x, y) => x.HasValue && y.HasValue)
					.ToReactiveCommand();
			this.AddResolutionFilterCommand
				.Subscribe(_ => {
					if (this.ResolutionWidth.Value is { } w && this.ResolutionHeight.Value is { } h) {
						this.Model.AddResolutionFilter(w, h);
					}
					this.ResolutionWidth.Value = null;
					this.ResolutionHeight.Value = null;
				})
				.AddTo(this.CompositeDisposable);

			// メディアタイプ
			this.MediaType.Value = this.MediaTypeList.First();
			this.AddMediaTypeFilterCommand.Subscribe(() => this.Model.AddMediaTypeFilter(this.MediaType.Value.Value));
			// 削除
			this.RemoveFilterCommand.Subscribe(this.Model.RemoveFilter).AddTo(this.CompositeDisposable);
		}
	}
}
