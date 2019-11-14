using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

using Livet;

using Microsoft.Data.Sqlite;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.States;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels;

using Unity;
using Unity.Lifetime;

namespace SandBeige.MediaBox {
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App {
		private ISettings _settings;
		private ILogging _logging;
		protected override void OnStartup(StartupEventArgs e) {
			var launchTime = DateTime.Now;

			var mutexName = @"Global\MediaBox_wY6SaWv6PDbq4zeZP";
			Mutex mutex;
			try {
				mutex = new Mutex(true, mutexName, out var createdNew);
				if (!createdNew) {
					MessageBox.Show("既に起動しています。");
					mutex.Close();
					return;
				}
			} catch {
				MessageBox.Show("既に起動しています。");
				return;
			}

			DispatcherHelper.UIDispatcher = this.Dispatcher;
			UIDispatcherScheduler.Initialize();
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;

			TypeRegistrations.RegisterType(new UnityContainer());

			this._logging = Get.Instance<ILogging>();
			this._logging.Log("ロガー取得");
			this._logging.Log($"起動時刻{launchTime:HH:mm:ss.fff}");

			var splashScreen = new Views.SplashScreen();
			splashScreen.Show();

			// 設定読み込み
			this._settings = Get.Instance<ISettings>();
			this._settings.Load();
			this._logging.Log("設定読み込み完了");

			// 状態読み込み
			var states = Get.Instance<States>();
			states.Load();
			this._logging.Log("状態読み込み完了");

			// FFME設定
			Unosquare.FFME.Library.FFmpegDirectory = this._settings.PathSettings.FfmpegDirectoryPath.Value;
			Unosquare.FFME.Library.FFmpegLoadModeFlags = FFmpeg.AutoGen.FFmpegLoadMode.FullFeatures;
			Unosquare.FFME.Library.EnableWpfMultiThreadedVideo = true;

			this._logging.Log("FFME設定完了");

			// ディレクトリがなければ作成
			if (!Directory.Exists(this._settings.PathSettings.ThumbnailDirectoryPath.Value)) {
				Directory.CreateDirectory(this._settings.PathSettings.ThumbnailDirectoryPath.Value);
				foreach (var i in Enumerable.Range(0, 256)) {
					Directory.CreateDirectory(Path.Combine(this._settings.PathSettings.ThumbnailDirectoryPath.Value, i.ToString("X2")));
				}
			}
			if (!Directory.Exists(this._settings.PathSettings.PluginDirectoryPath.Value)) {
				Directory.CreateDirectory(this._settings.PathSettings.PluginDirectoryPath.Value);
			}
			this._logging.Log("ディレクトリ作成完了");

			// DataBase
			var sb = new SqliteConnectionStringBuilder {
				DataSource = this._settings.PathSettings.DataBaseFilePath.Value
			};
			var dbContext = new MediaBoxDbContext(new SqliteConnection(sb.ConnectionString));
			dbContext.Database.EnsureCreated();
			// TODO : 暫定ファイル名
			UnityConfig.UnityContainer.RegisterInstance(new Database("MediaBox.Files.db"), new ContainerControlledLifetimeManager());
			UnityConfig.UnityContainer.RegisterInstance(dbContext, new ContainerControlledLifetimeManager());
			this._logging.Log("DB設定完了");


			// 画面起動
			this.MainWindow = new Views.MainWindow {
				DataContext = new MainWindowViewModel()
			};
			this._logging.Log("VM,メイン画面インスタンス作成完了");
			splashScreen.Close();

			this.MainWindow.ShowDialog();
			this._logging.Log("メイン画面終了");

			this._settings.Save();
			states.Save();
			this._logging.Log("設定保存完了");

			mutex.Close();
			mutex.Dispose();
		}

		/// <summary>
		/// 集約エラーハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			if (e.ExceptionObject is Exception ex) {
				this._logging.Log("集約エラーハンドラ", LogLevel.Warning, ex);
#if DEBUG
				// TODO:vs経由でデバッグ中に終了すると毎度例外が出てしまうので、応急処置
				if (ex.StackTrace.Contains("Microsoft.VisualStudio.DesignTools.WpfTap.Networking.ProtocolHandler.HandleMessage")) {
					Environment.Exit(1);
				}
#endif
			} else {
				this._logging.Log(e.ToString(), LogLevel.Warning);
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
