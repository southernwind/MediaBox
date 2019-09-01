using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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
		/// ファイルパスフィルター追加コマンド
		/// </summary>
		public ReactiveCommand<string> AddFilePathFilterCommand {
			get;
		} = new ReactiveCommand<string>();

		public FilePathFilterCreatorViewModel(FilteringCondition model) {
			this.ModelForToString = model;

			// ファイルパス
			this.AddFilePathFilterCommand.Subscribe(model.AddFilePathFilter).AddTo(this.CompositeDisposable);
		}
	}
}
