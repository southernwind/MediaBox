using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;

namespace SandBeige.MediaBox.ViewModels.Album.Filter.Creators {
	/// <summary>
	/// タグフィルター作成ViewModel
	/// </summary>
	internal class TagFilterCreatorViewModel : ViewModelBase, IFilterCreatorViewModel {
		/// <summary>
		/// 表示名
		/// </summary>
		public string Title {
			get {
				return "タグフィルター";
			}
		}

		/// <summary>
		/// タグ名
		/// </summary>
		public IReactiveProperty<string> TagName {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 検索条件として指定のタグを含むものを検索するか、含まないものを検索するかを選択する。
		/// </summary>
		public IReactiveProperty<BindingItem<SearchType>> SearchType {
			get;
		} = new ReactivePropertySlim<BindingItem<SearchType>>();

		/// <summary>
		/// 含む/含まないの選択候補
		/// </summary>
		public IEnumerable<BindingItem<SearchType>> SearchTypeList {
			get;
		} = new[] {
			new BindingItem<SearchType>("含む",Composition.Enum.SearchType.Include),
			new BindingItem<SearchType>("含まない",Composition.Enum.SearchType.Exclude)
		};

		/// <summary>
		/// タグフィルター追加コマンド
		/// </summary>
		public ReactiveCommand AddTagFilterCommand {
			get;
		}

		public TagFilterCreatorViewModel(FilteringCondition model) {
			this.ModelForToString = model;

			this.SearchType.Value = this.SearchTypeList.First();
			this.AddTagFilterCommand = this.TagName.Select(x => !string.IsNullOrEmpty(x)).ToReactiveCommand().AddTo(this.CompositeDisposable);
			this.AddTagFilterCommand.Subscribe(_ => model.AddTagFilter(this.TagName.Value, this.SearchType.Value.Value)).AddTo(this.CompositeDisposable);
		}
	}
}
