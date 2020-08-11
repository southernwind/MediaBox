using System.Collections.Generic;
using System.Linq;

using MahApps.Metro.IconPacks;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.ViewModels.Album.Filter.Creators {
	/// <summary>
	/// メディアタイプフィルター作成ViewModel
	/// </summary>
	public class MediaTypeFilterCreatorViewModel : ViewModelBase, IFilterCreatorViewModel {
		/// <summary>
		/// 表示名
		/// </summary>
		public string Title {
			get {
				return "メディアタイプフィルター";
			}
		}

		/// <summary>
		/// アイコン
		/// </summary>
		public PackIconBase Icon {
			get {
				return new PackIconFontAwesome { Kind = PackIconFontAwesomeKind.FileVideoRegular };
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

		public MediaTypeFilterCreatorViewModel(IFilteringCondition model) {
			this.ModelForToString = model;

			this.MediaType.Value = this.MediaTypeList.First();
			this.AddMediaTypeFilterCommand.Subscribe(() => model.AddMediaTypeFilter(this.MediaType.Value.Value));

		}
	}
}
