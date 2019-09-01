using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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
		/// タグフィルター追加コマンド
		/// </summary>
		public ReactiveCommand<string> AddTagFilterCommand {
			get;
		} = new ReactiveCommand<string>();

		public TagFilterCreatorViewModel(FilteringCondition model) {
			this.ModelForToString = model;

			this.AddTagFilterCommand.Subscribe(model.AddTagFilter).AddTo(this.CompositeDisposable);
		}
	}
}
