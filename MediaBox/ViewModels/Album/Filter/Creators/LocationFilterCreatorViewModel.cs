using System;
using System.Collections.Generic;
using System.Linq;

using MahApps.Metro.IconPacks;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;

namespace SandBeige.MediaBox.ViewModels.Album.Filter.Creators {
	/// <summary>
	/// 座標フィルター作成ViewModel
	/// </summary>
	public class LocationFilterCreatorViewModel : ViewModelBase, IFilterCreatorViewModel {
		/// <summary>
		/// 表示名
		/// </summary>
		public string Title {
			get {
				return "座標フィルター";
			}
		}

		/// <summary>
		/// アイコン
		/// </summary>
		public PackIconBase Icon {
			get {
				return new PackIconModern { Kind = PackIconModernKind.MapGps };
			}
		}

		/// <summary>
		/// メディアタイプフィルター追加コマンド
		/// </summary>
		public ReactiveCommand AddLocationFilterCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 座標情報を持っているか否か
		/// </summary>
		public IReactiveProperty<BindingItem<bool>> HasLocation {
			get;
		} = new ReactivePropertySlim<BindingItem<bool>>();

		/// <summary>
		/// 座標情報を持っているか否かの候補
		/// </summary>
		public IEnumerable<BindingItem<bool>> HasLocationList {
			get;
		} = new[] {
			new BindingItem<bool>("座標情報を含む",true),
			new BindingItem<bool>("座標情報を含まない",false)
		};

		public LocationFilterCreatorViewModel(FilteringCondition model) {
			this.ModelForToString = model;

			this.HasLocation.Value = this.HasLocationList.First();
			this.AddLocationFilterCommand.Subscribe(_ => {
				model.AddLocationFilter(this.HasLocation.Value.Value);
			});
		}
	}
}
