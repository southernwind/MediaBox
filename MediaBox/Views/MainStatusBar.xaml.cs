using System;
using System.Reactive.Linq;
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
				// メインウィンドウの移動とサイズの変更にポップアップウィンドウも追従する。
				Observable.FromEventPattern(
					x => mainWindow.SizeChanged += x.Invoke,
					x => mainWindow.SizeChanged -= x.Invoke)
					.Merge(Observable.FromEventPattern(
						x => mainWindow.LocationChanged += x.Invoke,
						x => mainWindow.LocationChanged -= x.Invoke))
					.Subscribe(_ => {
						// HorizontalOffsetを変更するとイベントが発生して自動的に位置を合わせてくれるみたい。
						var tqHo = this.TaskQueuePopup.HorizontalOffset;
						this.TaskQueuePopup.HorizontalOffset = tqHo + 0.0001;
						this.TaskQueuePopup.HorizontalOffset = tqHo;
						var nHo = this.NotificationPopup.HorizontalOffset;
						this.NotificationPopup.HorizontalOffset = nHo + 0.0001;
						this.NotificationPopup.HorizontalOffset = nHo;
						var lHo = this.LogPopup.HorizontalOffset;
						this.LogPopup.HorizontalOffset = lHo + 0.0001;
						this.LogPopup.HorizontalOffset = lHo;
					});
			}
		}
	}
}
