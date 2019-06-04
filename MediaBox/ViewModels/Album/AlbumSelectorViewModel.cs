using System;
using System.Reactive.Linq;
using System.Windows;

using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Gesture;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album.Filter;
using SandBeige.MediaBox.ViewModels.Album.Sort;
using SandBeige.MediaBox.ViewModels.Dialog;

namespace SandBeige.MediaBox.ViewModels.Album {
	/// <summary>
	/// アルバムセレクターViewModel
	/// </summary>
	internal class AlbumSelectorViewModel : ViewModelBase {
		/// <summary>
		/// モデル
		/// </summary>
		public AlbumSelector Model {
			get;
		}

		/// <summary>
		/// フィルターマネージャー
		/// </summary>
		public FilterDescriptionManagerViewModel FilterDescriptionManager {
			get;
		}

		/// <summary>
		/// ソートマネージャー
		/// </summary>
		public SortDescriptionManagerViewModel SortDescriptionManager {
			get;
		}

		/// <summary>
		/// キー操作受信
		/// </summary>
		public KeyGestureReceiver KeyGestureReceiver {
			get {
				return this.Model.KeyGestureReceiver;
			}
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
		/// フォルダアルバムフォルダパス
		/// </summary>
		public IReactiveProperty<string> FolderAlbumPath {
			get;
		}

		/// <summary>
		/// フォルダアルバムをカレントにするコマンド
		/// </summary>
		public ReactiveCommand SetFolderAlbumToCurrent {
			get;
		}

		/// <summary>
		/// アルバム作成ウィンドウオープンコマンド
		/// </summary>
		public ReactiveCommand OpenCreateAlbumWindowCommand {
			get;
		} = new ReactiveCommand();


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
		/// <param name="name">一意になる名称 フィルターとソート順の保存、復元に使用する。</param>
		public AlbumSelectorViewModel(AlbumSelector albumSelector) {
			this.Model = albumSelector;
			this.ModelForToString = this.Model;

			this.FilterDescriptionManager = Get.Instance<FilterDescriptionManagerViewModel>((FilterDescriptionManager)this.Model.FilterSetter);
			this.SortDescriptionManager = Get.Instance<SortDescriptionManagerViewModel>((SortDescriptionManager)this.Model.SortSetter);

			this.AlbumList = this.Model.AlbumList.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create, disposeElement: false);

			this.CurrentAlbum =
				this.Model
					.CurrentAlbum
					.Select(x => x == null ? null : this.ViewModelFactory.Create((AlbumModel)x))
					.ToReadOnlyReactiveProperty()
					.AddTo(this.CompositeDisposable);

			this.SetAlbumToCurrent.Subscribe(x => this.Model.SetAlbumToCurrent(x.Model)).AddTo(this.CompositeDisposable);

			this.FolderAlbumPath = this.Model.FolderAlbumPath.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.SetFolderAlbumToCurrent = this.FolderAlbumPath.Select(x => x != null).ToReactiveCommand().AddTo(this.CompositeDisposable);
			this.SetFolderAlbumToCurrent.Subscribe(this.Model.SetFolderAlbumToCurrent).AddTo(this.CompositeDisposable);

			this.OpenCreateAlbumWindowCommand.Subscribe(_ => {
				var vm = Get.Instance<AlbumEditorViewModel>();
				vm.CreateAlbumCommand.Execute();
				var message = new TransitionMessage(typeof(Views.Album.AlbumEditor), vm, TransitionMode.Normal);
				this.Messenger.Raise(message);
			}).AddTo(this.CompositeDisposable);

			this.OpenEditAlbumWindowCommand.Subscribe(x => {
				var vm = Get.Instance<AlbumEditorViewModel>();
				vm.EditAlbumCommand.Execute(x);
				var message = new TransitionMessage(typeof(Views.Album.AlbumEditor), vm, TransitionMode.Normal);
				this.Messenger.Raise(message);
			}).AddTo(this.CompositeDisposable);

			this.DeleteAlbumCommand.Subscribe(x => {
				if (x.Model is RegisteredAlbum ra) {
					using var vm = new DialogViewModel("確認", $"アルバム [ {x.Model.Title.Value} ] を削除します。", MessageBoxButton.OKCancel, MessageBoxResult.Cancel);
					var message = new TransitionMessage(vm, "ShowDialog");
					this.Messenger.Raise(message);
					if (vm.Result.Value == MessageBoxResult.OK) {
						this.Model.DeleteAlbum(ra);
					}
				}
			}).AddTo(this.CompositeDisposable);

			this.Shelf = this.Model.Shelf.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Folder = this.Model.Folder.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
		}
	}
}
