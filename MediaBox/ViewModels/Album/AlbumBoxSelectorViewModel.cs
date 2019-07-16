using System;
using System.Reactive.Linq;

using Livet.Messaging.Windows;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Album {

	/// <summary>
	/// アルバムボックス選択ViewModel
	/// </summary>
	internal class AlbumBoxSelectorViewModel : ViewModelBase {
		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReadOnlyReactiveProperty<int?> AlbumBoxId {
			get;
		}

		/// <summary>
		/// 編集が完了したか否か
		/// </summary>
		public bool Completed {
			get;
			private set;
		}

		/// <summary>
		/// 階層表示用アルバム格納棚
		/// </summary>
		public IReadOnlyReactiveProperty<AlbumBoxViewModel> Shelf {
			get;
		}

		/// <summary>
		/// 選択中アルバムボックス
		/// </summary>
		public IReactiveProperty<AlbumBoxViewModel> SelectedAlbumBox {
			get;
		} = new ReactivePropertySlim<AlbumBoxViewModel>();

		/// <summary>
		/// キャンセルコマンド
		/// </summary>
		public ReactiveCommand CancelCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンプリートコマンド
		/// </summary>
		public ReactiveCommand CompleteCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumBoxSelectorViewModel() {
			var model = Get.Instance<AlbumBoxSelector>().AddTo(this.CompositeDisposable);
			this.Shelf = model.Shelf.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.AlbumBoxId = this.SelectedAlbumBox.Select(x => x?.AlbumBoxId.Value).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.CompleteCommand.Subscribe(x => {
				this.Completed = true;
				this.Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
			});

			this.CancelCommand.Subscribe(x => {
				this.Completed = false;
				this.Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
			});
		}
	}
}
