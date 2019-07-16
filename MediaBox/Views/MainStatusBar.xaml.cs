using System.Windows;

namespace SandBeige.MediaBox.Views {
	/// <summary>
	/// MainStatusBar.xaml の相互作用ロジック
	/// </summary>
	public partial class MainStatusBar {
		public MainStatusBar() {
			this.InitializeComponent();
		}

		/// <summary>
		/// ステータスバー読み込み完了イベント
		/// </summary>
		/// <param name="sender">未使用</param>
		/// <param name="e">未使用</param>
		private void StatusBar_Loaded(object sender, RoutedEventArgs e) {
			var mainWindow = Window.GetWindow(this);
			if (mainWindow != null) {
				// メインウィンドウの移動にポップアップウィンドウも追従する。
				mainWindow.LocationChanged += (_, __) => {
					// HorizontalOffsetを変更するとイベントが発生して自動的に位置を合わせてくれるみたい。
					var ho = this.TaskQueuePopup.HorizontalOffset;
					this.TaskQueuePopup.HorizontalOffset = ho + 0.0001;
					this.TaskQueuePopup.HorizontalOffset = ho;
				};
			}
		}
	}
}
