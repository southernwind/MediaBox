using System.Windows;

using SandBeige.MediaBox.StyleChecker.ViewModels;

namespace SandBeige.MediaBox.StyleChecker {
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App {
		protected override void OnStartup(StartupEventArgs e) {
			this.MainWindow = new Views.MainWindow {
				DataContext = new MainWindowViewModel()
			};
			this.MainWindow.ShowDialog();
		}
	}
}
