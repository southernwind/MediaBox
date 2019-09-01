using System.Collections.Generic;
using System.Linq;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;

namespace SandBeige.MediaBox.ViewModels.Album.Filter.Creators {
	/// <summary>
	/// メディアタイプフィルター作成ViewModel
	/// </summary>
	internal class MediaTypeFilterCreatorViewModel : ViewModelBase, IFilterCreatorViewModel {
		/// <summary>
		/// 表示名
		/// </summary>
		public string Title {
			get {
				return "メディアタイプフィルター";
			}
		}

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

		public MediaTypeFilterCreatorViewModel(FilteringCondition model) {
			this.ModelForToString = model;

			this.MediaType.Value = this.MediaTypeList.First();
			this.AddMediaTypeFilterCommand.Subscribe(() => model.AddMediaTypeFilter(this.MediaType.Value.Value));

		}
	}
}
