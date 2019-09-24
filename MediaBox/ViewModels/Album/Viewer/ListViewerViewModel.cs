using System;

using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.ViewModels.Settings;
using SandBeige.MediaBox.Views.Settings;

namespace SandBeige.MediaBox.ViewModels.Album.Viewer {
	internal class ListViewerViewModel : ViewModelBase, IAlbumViewerViewModel {
		public IReactiveProperty<bool> IsSelected {
			get;
		} = new ReactivePropertySlim<bool>();

		public IAlbumViewModel AlbumViewModel {
			get;
		}

		/// <summary>
		/// 表示する列
		/// </summary>
		public ReadOnlyReactiveCollection<Col> Columns {
			get;
		}

		public ReactiveCommand OpenColumnSettingsWindowCommand {
			get;
		} = new ReactiveCommand();

		public ListViewerViewModel(IAlbumViewModel albumViewModel) {
			this.AlbumViewModel = albumViewModel;
			var model = new ListViewerModel(this.AlbumViewModel.AlbumModel);
			this.Columns = model.Columns.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			// 表示列設定ウィンドウ
			this.OpenColumnSettingsWindowCommand.Subscribe(_ => {
				var vm = new ColumnSettingsViewModel();
				var message = new TransitionMessage(typeof(ColumnSettingsWindow), vm, TransitionMode.NewOrActive);
				this.Messenger.Raise(message);
			}).AddTo(this.CompositeDisposable);
		}
	}
}
