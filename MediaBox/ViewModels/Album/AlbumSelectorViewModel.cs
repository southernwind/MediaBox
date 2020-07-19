using System;
using System.Reactive.Linq;
using System.Windows;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.ViewModels.Album.Filter;
using SandBeige.MediaBox.ViewModels.Album.Sort;
using SandBeige.MediaBox.ViewModels.Dialog;
using SandBeige.MediaBox.Views.Album;
using SandBeige.MediaBox.Views.Dialog;

using static SandBeige.MediaBox.ViewModels.Album.AlbumEditorWindowViewModel;

namespace SandBeige.MediaBox.ViewModels.Album {
	/// <summary>
	/// アルバムセレクターViewModel
	/// </summary>
	internal abstract class AlbumSelectorViewModel : ViewModelBase {
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
		/// アルバムリスト
		/// </summary>
		public ReadOnlyReactiveCollection<AlbumViewModel> AlbumList {
			get;
		}

		/// <summary>
		/// カレントアルバム
		/// </summary>
		public IReadOnlyReactiveProperty<AlbumViewModel> CurrentAlbum {
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
		public ReactiveCommand<AlbumViewModel> SetAlbumToCurrent {
			get;
		} = new ReactiveCommand<AlbumViewModel>();

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
		public AlbumSelectorViewModel(AlbumSelector albumSelector, IDialogService dialogService) {
			this.Model = albumSelector;
			this.ModelForToString = this.Model;

			this.FilterDescriptionManager = new FilterSelectorViewModel((FilterDescriptionManager)this.Model.FilterSetter, dialogService);
			this.SortDescriptionManager = new SortSelectorViewModel((SortDescriptionManager)this.Model.SortSetter, dialogService);

			this.AlbumList = this.Model.AlbumList.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create, disposeElement: false);

			this.CurrentAlbum =
				this.Model
					.CurrentAlbum
					.Select(x => x == null ? null : this.ViewModelFactory.Create((AlbumModel)x))
					.ToReadOnlyReactiveProperty()
					.AddTo(this.CompositeDisposable);

			this.SetAlbumToCurrent.Subscribe(x => this.Model.SetAlbumToCurrent(x.Model)).AddTo(this.CompositeDisposable);

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
				if (x.Model is RegisteredAlbum ra) {
					var param = new DialogParameters() {
						{CommonDialogWindowViewModel.ParameterNameTitle ,"確認" },
						{CommonDialogWindowViewModel.ParameterNameMessage ,$"アルバム [ {x.Model.Title.Value} ] を削除します。"},
						{CommonDialogWindowViewModel.ParameterNameButton ,MessageBoxButton.OKCancel },
						{CommonDialogWindowViewModel.ParameterNameDefaultButton ,MessageBoxResult.Cancel},
					};
					dialogService.ShowDialog(nameof(CommonDialogWindow), param, result => {
						if (result.Result == ButtonResult.OK) {
							this.Model.DeleteAlbum(ra);
						}
					});
				}
			}).AddTo(this.CompositeDisposable);

			this.Shelf = this.Model.Shelf.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Folder = this.Model.Folder.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
		}
	}

	/// <summary>
	/// メインウィンドウ用アルバムセレクターViewModel
	/// </summary>
	internal class MainAlbumSelectorViewModel : AlbumSelectorViewModel {
		public MainAlbumSelectorViewModel(MainAlbumSelector model, IDialogService dialogService) : base(model, dialogService) {

		}
	}

	/// <summary>
	/// 編集ウィンドウ用アルバムセレクターViewModel
	/// </summary>
	internal class EditorAlbumSelectorViewModel : AlbumSelectorViewModel {
		public EditorAlbumSelectorViewModel(EditorAlbumSelector model, IDialogService dialogService) : base(model, dialogService) {

		}
	}
}
