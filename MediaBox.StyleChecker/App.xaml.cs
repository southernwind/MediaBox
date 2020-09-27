using System;
using System.Windows;

using SandBeige.MediaBox.StyleChecker.ViewModels;

namespace SandBeige.MediaBox.StyleChecker {
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App {
		protected override void OnStartup(StartupEventArgs e) {
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
			this.MainWindow = new Views.MainWindow {
				DataContext = new MainWindowViewModel()
			};
			this.MainWindow.ShowDialog();
		}
		/// <summary>
		/// 集約エラーハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			if (e.ExceptionObject is Exception ex) {
				Console.WriteLine(ex);
#if DEBUG
				// TODO:vs経由でデバッグ中に終了すると毎度例外が出てしまうので、応急処置
				if (ex.StackTrace!.Contains("Microsoft.VisualStudio.DesignTools.WpfTap.Networking.ProtocolHandler.HandleMessage")) {
					Environment.Exit(1);
				}
#endif
			}

			MessageBox.Show(
				"不明なエラーが発生しました。アプリケーションを終了します。",
				"エラー",
				MessageBoxButton.OK,
				MessageBoxImage.Error);

			Environment.Exit(1);
		}
	}
}
