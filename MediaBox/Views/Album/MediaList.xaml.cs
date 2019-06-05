using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Views.Album {
	/// <summary>
	/// MediaList.xaml の相互作用ロジック
	/// </summary>
	public partial class MediaList {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaList() {
			this.InitializeComponent();
			this.DataContextChanged += (sender, e) => {
				if (!(this.DataContext is AlbumViewModel avm)) {
					return;
				}
				avm.CurrentIndex.Subscribe(x => {
					if (!(this.DataContext is AlbumViewModel avm)) {
						return;
					}
					if (avm.DisplayMode.Value != DisplayMode.Detail) {
						return;
					}
					this.SelectedItemPositionCentering();
				});
			};

			this.ListBox.SizeChanged += (sender, e) => {
				if (!(this.DataContext is AlbumViewModel avm)) {
					return;
				}
				this.SelectedItemPositionCentering();
			};
		}

		/// <summary>
		/// 選択中アイテムのセンタリング
		/// </summary>
		private async void SelectedItemPositionCentering() {
			while (true) {
				if (VisualTreeHelper.GetChildrenCount(this.ListBox) == 0) {
					return;
				}
				if (!(VisualTreeHelper.GetChild(this.ListBox, 0) is Border border)) {
					return;
				}

				if (!(border.Child is ScrollViewer scrollViewer)) {
					return;
				}

				if (!(this.DataContext is AlbumViewModel avm)) {
					return;
				}

				switch (avm.DisplayMode.Value) {
					case DisplayMode.Detail: {
						while (scrollViewer.ScrollableWidth == 0) {
							await Task.Delay(1);
							continue;
						}
						var listBoxWidth = scrollViewer.ViewportWidth;
						var index = this.ListBox.SelectedIndex;
						if (index == -1) {
							break;
						}
						var scrollOffset = index - (listBoxWidth / 2) + 0.5;
						if (scrollOffset > scrollViewer.ScrollableWidth) {
							scrollOffset = scrollViewer.ScrollableWidth;
						}

						scrollViewer.ScrollToHorizontalOffset(scrollOffset);
						break;
					}
					case DisplayMode.Library: {
						while (scrollViewer.ScrollableHeight == 0) {
							await Task.Delay(1);
							continue;
						}
						var listBoxHeight = scrollViewer.ViewportHeight;
						var index = this.ListBox.SelectedIndex;
						var max = this.ListBox.Items.Count;
						var adjust = listBoxHeight * ((index - (max / 2d)) / max);
						var scrollOffset = ((double)index / max * scrollViewer.ScrollableHeight) + adjust;
						if (scrollOffset > scrollViewer.ScrollableHeight) {
							scrollOffset = scrollViewer.ScrollableHeight;
						}

						scrollViewer.ScrollToVerticalOffset(scrollOffset);
						break;
					}
				}
				break;
			}
		}
	}
}
