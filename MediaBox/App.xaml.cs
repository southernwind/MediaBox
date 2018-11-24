using System;
using System.IO;
using System.Windows;

using Livet;
using Microsoft.Data.Sqlite;
using Reactive.Bindings;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels;
using Unity;
using Unity.Lifetime;

namespace SandBeige.MediaBox {
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application {
		private ISettings _settings;
		private ILogging _logging;
		protected override void OnStartup(StartupEventArgs e) {
			DispatcherHelper.UIDispatcher = this.Dispatcher;
			UIDispatcherScheduler.Initialize();
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;

			TypeRegistrations.RegisterType(new UnityContainer());

			this._logging = Get.Instance<ILogging>();

			// 設定読み込み
			this._settings = Get.Instance<ISettings>();
			this._settings.Load();

			// ディレクトリがなければ作成
			if (!Directory.Exists(this._settings.PathSettings.ThumbnailDirectoryPath.Value)) {
				Directory.CreateDirectory(this._settings.PathSettings.ThumbnailDirectoryPath.Value);
			}

			// DataBase
			var scsb = new SqliteConnectionStringBuilder {
				DataSource = this._settings.PathSettings.DataBaseFilePath.Value
			};
			var dbContext = new MediaBoxDbContext(new SqliteConnection(scsb.ConnectionString));
			dbContext.Database.EnsureCreated();
			UnityConfig.UnityContainer.RegisterInstance(dbContext, new ContainerControlledLifetimeManager());


			// 画面起動
			this.MainWindow = new Views.MainWindow() {
				DataContext = Get.Instance<MainWindowViewModel>()
			};

			this.MainWindow.ShowDialog();

			this._settings.Save();
		}

		/// <summary>
		/// 集約エラーハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			if (e.ExceptionObject is Exception ex) {
				this._logging.Log(LogLevel.Notice, "集約エラーハンドラ", ex);
			} else {
				this._logging.Log(LogLevel.Notice, e.ToString());
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
