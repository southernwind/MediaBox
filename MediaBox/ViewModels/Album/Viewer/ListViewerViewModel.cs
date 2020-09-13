using System;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.Views.Settings;

namespace SandBeige.MediaBox.ViewModels.Album.Viewer {
	public class ListViewerViewModel : ViewModelBase, IAlbumViewerViewModel {
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

		public ListViewerViewModel(IAlbumViewModel albumViewModel, IDialogService dialogService, ISettings settings) {
			this.AlbumViewModel = albumViewModel;
			var model = new ListViewerModel(settings);
			this.Columns = model.Columns.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			// 表示列設定ウィンドウ
			this.OpenColumnSettingsWindowCommand.Subscribe(_ => {
				dialogService.Show(nameof(ColumnSettingsWindow), null, null);
			}).AddTo(this.CompositeDisposable);
		}
	}
}
