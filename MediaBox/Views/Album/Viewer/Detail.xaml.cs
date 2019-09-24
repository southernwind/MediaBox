using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

using SandBeige.MediaBox.ViewModels.Album.Viewer;

namespace SandBeige.MediaBox.Views.Album.Viewer {
	/// <summary>
	/// Detail.xaml の相互作用ロジック
	/// </summary>
	public partial class Detail {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Detail() {
			this.InitializeComponent();
			this.DataContextChanged += (sender, e) => {
				if (!(this.DataContext is DetailViewerViewModel dvm)) {
					return;
				}
				dvm.AlbumViewModel.CurrentItem.ObserveOn(this.Dispatcher).Subscribe(x => {
					this.SelectedItemPositionCentering();
				});
			};

			this.ListBox.SizeChanged += (sender, e) => {
				if (!(this.DataContext is DetailViewerViewModel)) {
					return;
				}
				this.SelectedItemPositionCentering();
			};
		}
		/// <summary>
		/// 選択中アイテムのセンタリング
		/// </summary>
		private async void SelectedItemPositionCentering() {
			if (VisualTreeHelper.GetChildrenCount(this.ListBox) == 0) {
				return;
			}
			if (!(VisualTreeHelper.GetChild(this.ListBox, 0) is Border border)) {
				return;
			}

			if (!(border.Child is ScrollViewer scrollViewer)) {
				return;
			}

			if (!(this.DataContext is DetailViewerViewModel)) {
				return;
			}

			while (scrollViewer.ScrollableWidth == 0) {
				await Task.Delay(1);
			}
			var listBoxWidth = scrollViewer.ViewportWidth;
			var index = this.ListBox.SelectedIndex;
			if (index == -1) {
				return;
			}
			var scrollOffset = index - (listBoxWidth / 2) + 0.5;
			if (scrollOffset > scrollViewer.ScrollableWidth) {
				scrollOffset = scrollViewer.ScrollableWidth;
			}

			scrollViewer.ScrollToHorizontalOffset(scrollOffset);
		}
	}
}
