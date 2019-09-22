using System;
using System.Collections.Generic;
using System.ComponentModel;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;

namespace SandBeige.MediaBox.Composition.Interfaces.Implements {
	/// <summary>
	/// デザイナ用<see cref="IAlbumViewModel"/>実装
	/// </summary>
	[Obsolete("for designer")]
	public class AlbumViewModelForDesigner : IAlbumViewModel {
		public event PropertyChangedEventHandler PropertyChanged;
		public void Dispose() {
		}

		public ReadOnlyReactiveCollection<IMediaFileViewModel> Items {
			get;
		}

		public IAlbumModel AlbumModel {
			get;
		}

		public IGestureReceiver GestureReceiver {
			get;
		}

		public IReadOnlyReactiveProperty<string> Title {
			get;
		}

		public IReadOnlyReactiveProperty<long> ResponseTime {
			get;
		}

		public IReadOnlyReactiveProperty<int> BeforeFilteringCount {
			get;
		}

		public IReactiveProperty<IEnumerable<IMediaFileViewModel>> SelectedMediaFiles {
			get;
		}

		public IReadOnlyReactiveProperty<IMediaFileViewModel> CurrentItem {
			get;
		}

		public IReactiveProperty<DisplayMode> DisplayMode {
			get;
		}

		public IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		public ReactiveCommand<DisplayMode> ChangeDisplayModeCommand {
			get;
		}

		public ReactiveCommand<IEnumerable<IMediaFileViewModel>> AddMediaFileCommand {
			get;
		}

		public ReactiveCommand<IEnumerable<IMediaFileViewModel>> RemoveMediaFileCommand {
			get;
		}

		public ReactiveCommand OpenColumnSettingsWindowCommand {
			get;
		}

		public bool IsRegisteredAlbum {
			get;
		}

		public IReactiveProperty<string> Xaml {
			get;
		}
	}
}
