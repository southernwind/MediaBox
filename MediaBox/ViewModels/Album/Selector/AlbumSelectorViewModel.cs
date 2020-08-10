using System;
using System.Reactive.Linq;
using System.Windows;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.ViewModels.Album.Box;
using SandBeige.MediaBox.ViewModels.Album.Filter;
using SandBeige.MediaBox.ViewModels.Album.Sort;
using SandBeige.MediaBox.ViewModels.Dialog;
using SandBeige.MediaBox.Views.Album;
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
		public ReactiveCommand<IAlbumObject> SetAlbumToCurrent {
			get;
		} = new ReactiveCommand<IAlbumObject>();

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
		public ReactiveCommand<AlbumViewModel> OpenEditAlbumWindowCommand {
			get;
		} = new ReactiveCommand<AlbumViewModel>();

		/// <summary>
		/// アルバム削除コマンド
		/// </summary>
		public ReactiveCommand<AlbumViewModel> DeleteAlbumCommand {
			get;
		} = new ReactiveCommand<AlbumViewModel>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="albumSelector">モデル</param>
		public AlbumSelectorViewModel(IAlbumSelector albumSelector, IDialogService dialogService, IStates states, ViewModelFactory viewModelFactory) {
			this.ModelForToString = albumSelector;

			this.FilterDescriptionManager = new FilterSelectorViewModel((FilterDescriptionManager)albumSelector.FilterSetter, dialogService, states, viewModelFactory);
			this.SortDescriptionManager = new SortSelectorViewModel((SortDescriptionManager)albumSelector.SortSetter, dialogService, viewModelFactory);

			this.Album = viewModelFactory.Create(albumSelector.Album);

			this.SetAlbumToCurrent.Subscribe(x => albumSelector.SetAlbumToCurrent(x)).AddTo(this.CompositeDisposable);

			this.SetFolderAlbumToCurrent.Subscribe(albumSelector.SetFolderAlbumToCurrent).AddTo(this.CompositeDisposable);

			this.SetTagAlbumToCurrentCommand.Subscribe(albumSelector.SetDatabaseAlbumToCurrent);

			this.SetPlaceAlbumToCurrentCommand.Subscribe(albumSelector.SetPositionSearchAlbumToCurrent);

			this.SetWordSearchCommand.Subscribe(albumSelector.SetWordSearchAlbumToCurrent);

			this.OpenCreateAlbumWindowCommand.Subscribe(id => {
				var param = new DialogParameters {
					{ AlbumEditorModeToString(AlbumEditorMode.create), id }
				};
				dialogService.Show(nameof(AlbumEditorWindow), param, null);
			}).AddTo(this.CompositeDisposable);

			this.OpenEditAlbumWindowCommand.Subscribe(x => {
				var param = new DialogParameters {
					{ AlbumEditorModeToString(AlbumEditorMode.edit), x }
				};
				dialogService.Show(nameof(AlbumEditorWindow), param, null);
			}).AddTo(this.CompositeDisposable);

			this.DeleteAlbumCommand.Subscribe(x => {
				var param = new DialogParameters() {
						{CommonDialogWindowViewModel.ParameterNameTitle ,"確認" },
						{CommonDialogWindowViewModel.ParameterNameMessage ,$"アルバム [ {x.Model.Title.Value} ] を削除します。"},
						{CommonDialogWindowViewModel.ParameterNameButton ,MessageBoxButton.OKCancel },
						{CommonDialogWindowViewModel.ParameterNameDefaultButton ,MessageBoxResult.Cancel},
					};
				dialogService.ShowDialog(nameof(CommonDialogWindow), param, result => {
					if (result.Result == ButtonResult.OK) {
						albumSelector.DeleteAlbum(x.Model.AlbumObject);
					}
				});
			}).AddTo(this.CompositeDisposable);

			this.Shelf = albumSelector.Shelf.Select(viewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Folder = albumSelector.Folder.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
		}
	}
}
