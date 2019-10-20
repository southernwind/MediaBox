using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
namespace SandBeige.MediaBox.Plugins.Viewer.TileStyle {
	/// <summary>
	/// Viewer.xaml の相互作用ロジック
	/// </summary>
	public partial class Viewer {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Viewer() {
			this.InitializeComponent();

			this.ListBox.SizeChanged += (sender, e) => {
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

			while (scrollViewer.ScrollableHeight == 0) {
				await Task.Delay(1);
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
		}
	}
}
