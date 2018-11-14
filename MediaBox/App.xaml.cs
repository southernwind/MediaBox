using System;
using System.Windows;

using Livet;
using Reactive.Bindings;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.ViewModels;
using Unity;

namespace SandBeige.MediaBox {
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application {
		protected override void OnStartup(StartupEventArgs e) {
			DispatcherHelper.UIDispatcher = this.Dispatcher;
			UIDispatcherScheduler.Initialize();
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;

			TypeRegistrations.RegisterType(new UnityContainer());
			this.MainWindow = new Views.MainWindow() {
				DataContext = UnityConfig.UnityContainer.Resolve<MainWindowViewModel>()
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
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine(ex.Message);
			} else {
				Console.WriteLine(e.ToString());
			}

			//TODO:ロギング処理など
			MessageBox.Show(
				"不明なエラーが発生しました。アプリケーションを終了します。",
				"エラー",
				MessageBoxButton.OK,
				MessageBoxImage.Error);

			Environment.Exit(1);
		}
	}
}
