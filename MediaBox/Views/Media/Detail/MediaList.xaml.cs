using System.Windows.Controls;
using System.Windows.Input;

namespace SandBeige.MediaBox.Views.Media.Detail {
	/// <summary>
	/// MediaList.xaml の相互作用ロジック
	/// </summary>
	public partial class MediaList {
		public MediaList() {
			this.InitializeComponent();
		}

		private void ListBoxEx_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			if (!(sender is ListBox listBox)) {
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
	}
}
