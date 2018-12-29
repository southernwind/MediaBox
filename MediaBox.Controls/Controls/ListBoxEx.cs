using System.Windows.Controls;

namespace SandBeige.MediaBox.Controls.Controls {
	public class ListBoxEx : ListBox {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListBoxEx() {
			this.SelectionChanged += (sender, e) => {
				if (this.SelectedItems.Count == 1) {
					this.ScrollIntoView(this.SelectedItem);
				}
			};

			this.IsVisibleChanged += (sender, e) => {
				this.ScrollIntoView(this.SelectedItem);
			};
		}

	}
}
