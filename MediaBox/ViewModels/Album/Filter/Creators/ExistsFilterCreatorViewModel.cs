using System;
using System.Collections.Generic;
using System.Linq;

using MahApps.Metro.IconPacks;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;

namespace SandBeige.MediaBox.ViewModels.Album.Filter.Creators {
	/// <summary>
	/// 存在フィルター作成ViewModel
	/// </summary>
	public class ExistsFilterCreatorViewModel : ViewModelBase, IFilterCreatorViewModel {
		/// <summary>
		/// 表示名
		/// </summary>
		public string Title {
			get {
				return "存在フィルター";
			}
		}

		/// <summary>
		/// アイコン
		/// </summary>
		public PackIconBase Icon {
			get {
				return new PackIconMaterial { Kind = PackIconMaterialKind.FileHidden };
			}
		}

		/// <summary>
		/// ファイルが存在するか否か
		/// </summary>
		public IReactiveProperty<BindingItem<bool>> Exists {
			get;
		} = new ReactivePropertySlim<BindingItem<bool>>();

		/// <summary>
		/// ファイルが存在するか否かの候補
		/// </summary>
		public IEnumerable<BindingItem<bool>> ExistsList {
			get;
		} = new[] {
			new BindingItem<bool>("ファイルが存在する",true),
			new BindingItem<bool>("ファイルが存在しない",false)
		};

		/// <summary>
		/// ファイル存在フィルター追加コマンド
		/// </summary>
		public ReactiveCommand AddExistsFilterCommand {
			get;
		} = new ReactiveCommand();

		public ExistsFilterCreatorViewModel(FilteringCondition model) {
			this.ModelForToString = model;

			this.Exists.Value = this.ExistsList.First();
			this.AddExistsFilterCommand.Subscribe(_ => {
				model.AddExistsFilter(this.Exists.Value.Value);
			});
		}
	}
}
