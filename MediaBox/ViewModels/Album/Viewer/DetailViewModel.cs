using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.ViewModels.Album.Viewer {
	internal class DetailViewModel : ViewModelBase {
		public IReactiveProperty<bool> IsSelected {
			get;
		} = new ReactivePropertySlim<bool>();

		public IAlbumViewModel AlbumViewModel {
			get;
		}

		public DetailViewModel(IAlbumViewModel albumViewModel) {
			var albumModel = albumViewModel.AlbumModel;
			this.AlbumViewModel = albumViewModel;

			// 先読みロード
			albumModel.CurrentMediaFile
				.CombineLatest(
					this.IsSelected,
					(file, isSelected) => (file, isSelected))
				.Where(x => x.file != null)
				.Subscribe(x => {
					using (this.DisposeLock.DisposableEnterReadLock()) {
						if (this.DisposeState != Composition.Enum.DisposeState.NotDisposed) {
							return;
						}
						if (!x.isSelected) {
							// 全アンロード
							albumModel.Prefetch(Array.Empty<IMediaFileModel>());
							return;
						}

						var index = albumModel.Items.IndexOf(x.file);

						var minIndex = Math.Max(0, index - 2);
						IEnumerable<IMediaFileModel> models;
						lock (albumModel.Items) {
							var count = Math.Min(index + 2, albumModel.Items.Count - 1) - minIndex + 1;
							// 読み込みたい順に並べる
							models =
								Enumerable
									.Range(minIndex, count)
									.OrderBy(i => i >= index ? 0 : 1)
									.ThenBy(i => Math.Abs(i - index))
									.Select(i => albumModel.Items[i])
									.ToArray();
						}
						albumModel.Prefetch(models);
					}
				});

			albumModel.GestureReceiver
				.KeyEvent
				.Subscribe(x => {
					if (!this.IsSelected.Value) {
						return;
					}
					switch (x.Key) {
						case Key.Left:
							if (x.IsDown) {
								albumModel.SelectPreviewItem();
							}
							break;
						case Key.Right:
							if (x.IsDown) {
								albumModel.SelectNextItem();
							}
							break;

					}
				}).AddTo(this.CompositeDisposable);

			albumModel.GestureReceiver
				.MouseWheelEvent
				.Where(_ => !albumModel.GestureReceiver.IsControlKeyPressed)
				.Subscribe(x => {
					if (!this.IsSelected.Value) {
						return;
					}
					if (x.Delta > 0) {
						albumModel.SelectPreviewItem();
					} else {
						albumModel.SelectNextItem();
					}
				}).AddTo(this.CompositeDisposable);
		}
	}
}
