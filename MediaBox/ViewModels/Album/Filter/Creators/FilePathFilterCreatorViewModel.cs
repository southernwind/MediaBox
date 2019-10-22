using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using MahApps.Metro.IconPacks;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;

namespace SandBeige.MediaBox.ViewModels.Album.Filter.Creators {
	/// <summary>
	/// ファイルパスフィルター作成ViewModel
	/// </summary>
	internal class FilePathFilterCreatorViewModel : ViewModelBase, IFilterCreatorViewModel {
		/// <summary>
		/// 表示名
		/// </summary>
		public string Title {
			get {
				return "ファイルパスフィルター";
			}

		}

		/// <summary>
		/// アイコン
		/// </summary>
		public PackIconBase Icon {
			get {
				return new PackIconMaterialLight { Kind = PackIconMaterialLightKind.File };
			}
		}

		/// <summary>
		/// タグ名
		/// </summary>
		public IReactiveProperty<string> FilePath {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 検索条件として指定のタグを含むものを検索するか、含まないものを検索するかを選択する。
		/// </summary>
		public IReactiveProperty<BindingItem<SearchTypeInclude>> SearchType {
			get;
		} = new ReactivePropertySlim<BindingItem<SearchTypeInclude>>();

		/// <summary>
		/// 含む/含まないの選択候補
		/// </summary>
		public IEnumerable<BindingItem<SearchTypeInclude>> SearchTypeList {
			get;
		} = new[] {
			new BindingItem<SearchTypeInclude>("含む",SearchTypeInclude.Include),
			new BindingItem<SearchTypeInclude>("含まない",SearchTypeInclude.Exclude)
		};

		/// <summary>
		/// ファイルパスフィルター追加コマンド
		/// </summary>
		public ReactiveCommand AddFilePathFilterCommand {
			get;
		}

		public FilePathFilterCreatorViewModel(FilteringCondition model) {
			this.ModelForToString = model;

			this.SearchType.Value = this.SearchTypeList.First();
			this.AddFilePathFilterCommand = this.FilePath.Select(x => !string.IsNullOrEmpty(x)).ToReactiveCommand();
			this.AddFilePathFilterCommand.Subscribe(_ => model.AddFilePathFilter(this.FilePath.Value, this.SearchType.Value.Value)).AddTo(this.CompositeDisposable);
		}
	}
}
