using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Navigation;

namespace SandBeige.MediaBox.Controls.Behaviors {
	/// <summary>
	/// ハイパーリンクナビゲートリクエスト時ブラウザ起動ビヘイビア
	/// </summary>
	public class HyperlinkNavigateBehavior : Behavior<Hyperlink> {
		/// <summary>
		/// アタッチ
		/// </summary>
		protected override void OnAttached() {
			base.OnAttached();
			this.AssociatedObject.RequestNavigate += Navigate;
		}

		/// <summary>
		/// デタッチ
		/// </summary>
		protected override void OnDetaching() {
			base.OnDetaching();
			this.AssociatedObject.RequestNavigate -= Navigate;
		}

		/// <summary>
		/// ナビゲート
		/// </summary>
		/// <param name="sender">未使用</param>
		/// <param name="e">イベント引数</param>
		private static void Navigate(object sender, RequestNavigateEventArgs e) {
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}
