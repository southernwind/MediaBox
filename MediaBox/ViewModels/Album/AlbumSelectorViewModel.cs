using System;
using System.Reactive.Linq;

using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Album {
	/// <summary>
	/// アルバムコンテナViewModel
	/// </summary>
	internal class AlbumSelectorViewModel : ViewModelBase {
		/// <summary>
		/// アルバム一覧
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
		/// 引数のアルバムをカレントにするコマンド
		/// </summary>
		public ReactiveCommand<AlbumViewModel> SetAlbumToCurrent {
			get;
		} = new ReactiveCommand<AlbumViewModel>();

		/// <summary>
		/// 一時アルバムフォルダパス
		/// </summary>
		public IReactiveProperty<string> TemporaryAlbumPath {
			get;
		}

		/// <summary>
		/// 一時アルバムをカレントにするコマンド
		/// </summary>
		public ReactiveCommand SetTemporaryAlbumToCurrent {
			get;
		}

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
		public AlbumSelectorViewModel() {
			var model = Get.Instance<AlbumSelector>().AddTo(this.CompositeDisposable);

			this.AlbumList = model.AlbumList.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create, disposeElement: false);

			this.CurrentAlbum =
				model
					.CurrentAlbum
					.Select(x => x == null ? null : this.ViewModelFactory.Create(x))
					.ToReadOnlyReactiveProperty()
					.AddTo(this.CompositeDisposable);

			this.SetAlbumToCurrent.Subscribe(x => model.SetAlbumToCurrent(x.Model)).AddTo(this.CompositeDisposable);

			this.TemporaryAlbumPath = model.TemporaryAlbumPath.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.SetTemporaryAlbumToCurrent = this.TemporaryAlbumPath.Select(x => x != null).ToReactiveCommand().AddTo(this.CompositeDisposable);
			this.SetTemporaryAlbumToCurrent.Subscribe(model.SetTemporaryAlbumToCurrent).AddTo(this.CompositeDisposable);

			this.OpenEditAlbumWindowCommand.Subscribe(x => {
				var vm = Get.Instance<AlbumCreatorViewModel>();
				vm.EditAlbumCommand.Execute(x);
				var message = new TransitionMessage(typeof(Views.SubWindows.AlbumCreateWindow.AlbumCreateWindow), vm, TransitionMode.Normal);
				this.Messenger.Raise(message);
			}).AddTo(this.CompositeDisposable);

			this.DeleteAlbumCommand.Subscribe(x => {
				if (x.Model is RegisteredAlbum ra) {
					model.DeleteAlbum(ra);
				}
			}).AddTo(this.CompositeDisposable);

			this.Shelf = model.Shelf.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
		}
	}
}
