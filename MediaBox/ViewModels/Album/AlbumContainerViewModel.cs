using System;
using System.Linq;
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
	internal class AlbumContainerViewModel : ViewModelBase {
		/// <summary>
		/// モデル
		/// </summary>
		private readonly AlbumContainer _model;

		/// <summary>
		/// アルバム一覧
		/// </summary>
		public ReadOnlyReactiveCollection<AlbumViewModel> AlbumList {
			get;
		}

		/// <summary>
		/// カレントアルバム
		/// </summary>
		public ReadOnlyReactiveProperty<AlbumViewModel> CurrentAlbum {
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
		public ReactiveProperty<string> TemporaryAlbumPath {
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
		public AlbumContainerViewModel() {
			this._model = Get.Instance<AlbumContainer>();

			this.AlbumList = this._model.AlbumList.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create);

			this.CurrentAlbum =
				this._model
					.CurrentAlbum
					.Select(x => x == null ? null : this.ViewModelFactory.Create(x))
					.ToReadOnlyReactiveProperty();

			this.SetAlbumToCurrent.Subscribe(x => this._model.SetAlbumToCurrent(x.Model));

			this.TemporaryAlbumPath = this._model.TemporaryAlbumPath.ToReactivePropertyAsSynchronized(x => x.Value);

			this.SetTemporaryAlbumToCurrent = this.TemporaryAlbumPath.Select(x => x != null).ToReactiveCommand();
			this.SetTemporaryAlbumToCurrent.Subscribe(this._model.SetTemporaryAlbumToCurrent);

			this.OpenEditAlbumWindowCommand.Subscribe(x => {
				using (var vm = Get.Instance<AlbumCreatorViewModel>()) {
					vm.EditAlbumCommand.Execute(x);
					var message = new TransitionMessage(typeof(Views.SubWindows.AlbumCreateWindow.AlbumCreateWindow), vm, TransitionMode.NewOrActive);
					this.Messenger.Raise(message);
				}
			});

			this.DeleteAlbumCommand.Subscribe(x => {
				if (x.Model is RegisteredAlbum ra) {
					this._model.DeleteAlbum(ra);
				}
			});
		}
	}
}
