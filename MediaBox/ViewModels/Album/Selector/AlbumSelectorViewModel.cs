using System;
using System.Reactive.Linq;
using System.Windows;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Box;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Selector;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.States;
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
	public abstract class AlbumSelectorViewModel : ViewModelBase {
		/// <summary>
		/// モデル
		/// </summary>
		public AlbumSelector Model {
			get;
		}

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
		public IReadOnlyReactiveProperty<FolderObject> Folder {
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
		public AlbumSelectorViewModel(AlbumSelector albumSelector, IDialogService dialogService, States states, ViewModelFactory viewModelFactory) {
			this.Model = albumSelector;
			this.ModelForToString = this.Model;

			this.FilterDescriptionManager = new FilterSelectorViewModel((FilterDescriptionManager)this.Model.FilterSetter, dialogService, states, viewModelFactory);
			this.SortDescriptionManager = new SortSelectorViewModel((SortDescriptionManager)this.Model.SortSetter, dialogService, viewModelFactory);

			this.Album = viewModelFactory.Create(this.Model.Album as AlbumModel);

			this.SetAlbumToCurrent.Subscribe(x => this.Model.SetAlbumToCurrent(x)).AddTo(this.CompositeDisposable);

			this.SetFolderAlbumToCurrent.Subscribe(this.Model.SetFolderAlbumToCurrent).AddTo(this.CompositeDisposable);

			this.SetTagAlbumToCurrentCommand.Subscribe(this.Model.SetDatabaseAlbumToCurrent);

			this.SetPlaceAlbumToCurrentCommand.Subscribe(this.Model.SetPositionSearchAlbumToCurrent);

			this.SetWordSearchCommand.Subscribe(this.Model.SetWordSearchAlbumToCurrent);

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
						this.Model.DeleteAlbum(x.Model.AlbumObject);
					}
				});
			}).AddTo(this.CompositeDisposable);

			this.Shelf = this.Model.Shelf.Select(x => viewModelFactory.Create(x as AlbumBox)).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Folder = this.Model.Folder.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
		}
	}

	/// <summary>
	/// メインウィンドウ用アルバムセレクターViewModel
	/// </summary>
	public class MainAlbumSelectorViewModel : AlbumSelectorViewModel {
		public MainAlbumSelectorViewModel(MainAlbumSelector model, IDialogService dialogService, States states, ViewModelFactory viewModelFactory) : base(model, dialogService, states, viewModelFactory) {

		}
	}

	/// <summary>
	/// 編集ウィンドウ用アルバムセレクターViewModel
	/// </summary>
	public class EditorAlbumSelectorViewModel : AlbumSelectorViewModel {
		public EditorAlbumSelectorViewModel(EditorAlbumSelector model, IDialogService dialogService, States states, ViewModelFactory viewModelFactory) : base(model, dialogService, states, viewModelFactory) {

		}
	}
}
