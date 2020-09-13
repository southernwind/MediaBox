using System;
using System.Reactive.Linq;
using System.Windows;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.ViewModels.Album.Box;
using SandBeige.MediaBox.ViewModels.Album.Filter;
using SandBeige.MediaBox.ViewModels.Album.Sort;
using SandBeige.MediaBox.ViewModels.Dialog;
using SandBeige.MediaBox.Views.Album.Editor;
using SandBeige.MediaBox.Views.Dialog;

using static SandBeige.MediaBox.ViewModels.Album.Editor.AlbumEditorWindowViewModel;

namespace SandBeige.MediaBox.ViewModels.Album.Selector {
	/// <summary>
	/// アルバムセレクターViewModel
	/// </summary>
	public class AlbumSelectorViewModel : ViewModelBase {

		/// <summary>
		/// フィルターマネージャー
		/// </summary>
		public FilterSelectorViewModel FilterDescriptionManager {
			get;
		}

		/// <summary>
		/// ソートマネージャー
		/// </summary>
		public SortSelectorViewModel SortDescriptionManager {
			get;
		}

		/// <summary>
		/// カレントアルバム
		/// </summary>
		public AlbumViewModel Album {
			get;
		}

		/// <summary>
		/// 階層表示用アルバム格納棚
		/// </summary>
		public IReadOnlyReactiveProperty<AlbumBoxViewModel> Shelf {
			get;
		}

		/// <summary>
		/// Folder
		/// </summary>
		public IReadOnlyReactiveProperty<IAlbumSelectorFolderObject> Folder {
			get;
		}


		/// <summary>
		/// 引数のアルバムをカレントにするコマンド
		/// </summary>
		public ReactiveCommand<IAlbumObject?> SetAlbumToCurrent {
			get;
		} = new ReactiveCommand<IAlbumObject?>();

		/// <summary>
		/// フォルダアルバムをカレントにするコマンド
		/// </summary>
		public ReactiveCommand<string> SetFolderAlbumToCurrent {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// ワード検索コマンド
		/// </summary>
		public ReactiveCommand<string> SetWordSearchCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// アルバム作成ウィンドウオープンコマンド
		/// </summary>
		public ReactiveCommand<int?> OpenCreateAlbumWindowCommand {
			get;
		} = new ReactiveCommand<int?>();

		/// <summary>
		/// タグアルバムオープンコマンド
		/// </summary>
		public ReactiveCommand<string> SetTagAlbumToCurrentCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// 場所検索アルバムオープンコマンド
		/// </summary>
		public ReactiveCommand<Address> SetPlaceAlbumToCurrentCommand {
			get;
		} = new ReactiveCommand<Address>();

		/// <summary>
		/// アルバム編集ウィンドウオープンコマンド
		/// </summary>
		public ReactiveCommand<AlbumForBoxViewModel> OpenEditAlbumWindowCommand {
			get;
		} = new ReactiveCommand<AlbumForBoxViewModel>();

		/// <summary>
		/// アルバム削除コマンド
		/// </summary>
		public ReactiveCommand<AlbumForBoxViewModel> DeleteAlbumCommand {
			get;
		} = new ReactiveCommand<AlbumForBoxViewModel>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="albumSelector">モデル</param>
		public AlbumSelectorViewModel(IAlbumSelector albumSelector, IDialogService dialogService, IStates states, ViewModelFactory viewModelFactory) {
			this.ModelForToString = albumSelector;

			this.FilterDescriptionManager = new FilterSelectorViewModel(albumSelector.FilterSetter, dialogService, states, viewModelFactory);
			this.SortDescriptionManager = new SortSelectorViewModel(albumSelector.SortSetter, dialogService, viewModelFactory);

			this.Album = viewModelFactory.Create(albumSelector.Album);

			this.SetAlbumToCurrent.Subscribe(albumSelector.SetAlbumToCurrent).AddTo(this.CompositeDisposable);

			this.SetFolderAlbumToCurrent.Subscribe(albumSelector.SetFolderAlbumToCurrent).AddTo(this.CompositeDisposable);

			this.SetTagAlbumToCurrentCommand.Subscribe(albumSelector.SetDatabaseAlbumToCurrent);

			this.SetPlaceAlbumToCurrentCommand.Subscribe(albumSelector.SetPositionSearchAlbumToCurrent);

			this.SetWordSearchCommand.Subscribe(albumSelector.SetWordSearchAlbumToCurrent);

			this.OpenCreateAlbumWindowCommand.Subscribe(id => {
				var param = new DialogParameters {
					{ AlbumEditorModeToString(AlbumEditorMode.Create), id }
				};
				dialogService.Show(nameof(AlbumEditorWindow), param, null);
			}).AddTo(this.CompositeDisposable);

			this.OpenEditAlbumWindowCommand.Subscribe(x => {
				var param = new DialogParameters {
					{ AlbumEditorModeToString(AlbumEditorMode.Edit), x.AlbumObject }
				};
				dialogService.Show(nameof(AlbumEditorWindow), param, null);
			}).AddTo(this.CompositeDisposable);

			this.DeleteAlbumCommand.Subscribe(x => {
				var param = new DialogParameters() {
						{CommonDialogWindowViewModel.ParameterNameTitle ,"確認" },
						{CommonDialogWindowViewModel.ParameterNameMessage ,$"アルバム [ {x.Title.Value} ] を削除します。"},
						{CommonDialogWindowViewModel.ParameterNameButton ,MessageBoxButton.OKCancel },
						{CommonDialogWindowViewModel.ParameterNameDefaultButton ,MessageBoxResult.Cancel},
					};
				dialogService.ShowDialog(nameof(CommonDialogWindow), param, result => {
					if (result.Result == ButtonResult.OK) {
						albumSelector.DeleteAlbum(x.AlbumObject);
					}
				});
			}).AddTo(this.CompositeDisposable);

			this.Shelf = albumSelector.Shelf.Select(viewModelFactory.Create).ToReadOnlyReactivePropertySlim(null!).AddTo(this.CompositeDisposable);
			this.Folder = albumSelector.Folder.ToReadOnlyReactivePropertySlim(null!).AddTo(this.CompositeDisposable);
		}
	}
}
