using System;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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
		/// 評価フィルター追加コマンド
		/// </summary>
		public ReactiveCommand AddRateFilterCommand {
			get;
		}

		public RateFilterCreatorViewModel(FilteringCondition model) {
			this.ModelForToString = model;

			this.AddRateFilterCommand = this.Rate.Select(x => x.HasValue).ToReactiveCommand();
			this.AddRateFilterCommand
				.Subscribe(_ => {
					if (this.Rate.Value is { } r) {
						model.AddRateFilter(r);
					}

					this.Rate.Value = null;
				})
				.AddTo(this.CompositeDisposable);

		}
	}
}
