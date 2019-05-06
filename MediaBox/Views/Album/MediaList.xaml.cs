using System.Windows.Controls;
using System.Windows.Input;
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
			this.ListBox.SelectionChanged += (sender, e) => {
				if (this.ListBox.SelectedItems.Count == 1) {
					this.SelectedItemPositionCentering();
				}
			};

			this.ListBox.Loaded += (sender, e) => {
				this.SelectedItemPositionCentering();
			};

			this.ListBox.SizeChanged += (sender, e) => {
				this.SelectedItemPositionCentering();
			};
		}

		/// <summary>
		/// マウススクロール時
		/// </summary>
		/// <param name="sender">発火コントロール</param>
		/// <param name="e">イベント引数</param>
		private void ListBoxEx_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			if (!(sender is ListBox listBox)) {
				return;
			}
			if (!(this.DataContext is AlbumViewModel avm) || avm.DisplayMode.Value != DisplayMode.Detail) {
				return;
			}

			if (e.Delta < 0) {
				if (listBox.SelectedIndex < listBox.Items.Count - 1) {
					listBox.SelectedIndex += 1;
				}
			} else {
				if (listBox.SelectedIndex > 0) {
					listBox.SelectedIndex -= 1;
				}
			}
		}

		/// <summary>
		/// 選択中アイテムのセンタリング
		/// </summary>
		private void SelectedItemPositionCentering() {
			try {
				if (!(VisualTreeHelper.GetChild(this.ListBox, 0) is Border border)) {
					return;
				}

				if (!(border.Child is ScrollViewer scrollViewer)) {
					return;
				}

				var listBoxWidth = scrollViewer.ViewportWidth;
				var index = this.ListBox.SelectedIndex;
				var scrollOffset = index - (listBoxWidth / 2) + 0.5;
				if (scrollOffset > scrollViewer.ScrollableWidth) {
					scrollOffset = scrollViewer.ScrollableWidth;
				}

				scrollViewer.ScrollToHorizontalOffset(scrollOffset);
			} catch {
				// 何らかの例外。ここで起きる例外は大きな問題にはならないのでとりあえず放置
			}
		}
	}
}
