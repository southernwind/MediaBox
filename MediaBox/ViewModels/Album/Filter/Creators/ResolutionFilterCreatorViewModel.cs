using System;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album.Filter;

namespace SandBeige.MediaBox.ViewModels.Album.Filter.Creators {
	/// <summary>
	/// 解像度フィルター作成ViewModel
	/// </summary>
	internal class ResolutionFilterCreatorViewModel : ViewModelBase, IFilterCreatorViewModel {
		/// <summary>
		/// 表示名
		/// </summary>
		public string Title {
			get {
				return "解像度フィルター";
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
		public IReactiveProperty<int?> ResolutionWidth {
			get;
		} = new ReactivePropertySlim<int?>();

		/// <summary>
		/// 解像度高さ
		/// </summary>
		public IReactiveProperty<int?> ResolutionHeight {
			get;
		} = new ReactivePropertySlim<int?>();

		public ResolutionFilterCreatorViewModel(FilteringCondition model) {
			this.ModelForToString = model;

			this.AddResolutionFilterCommand =
				this.ResolutionWidth
					.CombineLatest(this.ResolutionHeight, (x, y) => x.HasValue && y.HasValue)
					.ToReactiveCommand();
			this.AddResolutionFilterCommand
				.Subscribe(_ => {
					if (this.ResolutionWidth.Value is { } w && this.ResolutionHeight.Value is { } h) {
						model.AddResolutionFilter(w, h);
					}
					this.ResolutionWidth.Value = null;
					this.ResolutionHeight.Value = null;
				})
				.AddTo(this.CompositeDisposable);
		}
	}
}
