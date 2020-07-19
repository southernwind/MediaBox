using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

using Microsoft.Data.Sqlite;

using Prism.Ioc;
using Prism.Unity;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.History;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.Models.Gesture;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Notification;
using SandBeige.MediaBox.Models.Plugin;
using SandBeige.MediaBox.Models.Settings;
using SandBeige.MediaBox.Models.States;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.Views;
using SandBeige.MediaBox.Views.Album;
using SandBeige.MediaBox.Views.Map;
using SandBeige.MediaBox.Views.Utils;

using Unity;
using Unity.Injection;
using Unity.Lifetime;


namespace SandBeige.MediaBox {
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App {
		private ISettings _settings;
		private ILogging _logging;
		private Mutex _mutex;
		private States _states;
		private Views.SplashScreen _splashScreen;

		/// <summary>
		/// 初期ウィンドウ作成
		/// </summary>
		/// <returns></returns>
		protected override Window CreateShell() {
			return this.Container.Resolve<MainWindow>();
		}

		/// <summary>
		/// 初期処理
		/// </summary>
		protected override void Initialize() {
			var mutexName = @"Global\MediaBox_wY6SaWv6PDbq4zeZP";
			try {
				this._mutex = new Mutex(true, mutexName, out var createdNew);
				if (!createdNew) {
					MessageBox.Show("既に起動しています。");
					this._mutex.Close();
					return;
				}
			} catch {
				MessageBox.Show("既に起動しています。");
				return;
			}

			UIDispatcherScheduler.Initialize();
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
			this._splashScreen = new Views.SplashScreen();
			this._splashScreen.Show();
			base.Initialize();

		}

		/// <summary>
		/// 初期処理終了後処理
		/// </summary>
		protected override void OnInitialized() {
			base.OnInitialized();
			this._logging.Log("VM,メイン画面インスタンス作成完了");
			this._splashScreen.Close();

		}

		/// <summary>
		/// DIコンテナ登録
		/// </summary>
		/// <param name="containerRegistry">コンテナ</param>
		protected override void RegisterTypes(IContainerRegistry containerRegistry) {
			base.RegisterRequiredTypes(containerRegistry);

			var launchTime = DateTime.Now;

			// ロガー
			containerRegistry.RegisterSingleton<ILogging, Logging>();

			// 設定
			containerRegistry.GetContainer().RegisterType<ISettings, Settings>(
				new ContainerControlledLifetimeManager(),
				new InjectionConstructor(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.settings"))
			);
			containerRegistry.Register<IGeneralSettings, GeneralSettings>();
			containerRegistry.Register<IPathSettings, PathSettings>();
			containerRegistry.Register<IScanSettings, ScanSettings>();
			containerRegistry.Register<IViewerSettings, ViewerSettings>();
			containerRegistry.Register<IPluginSettings, PluginSettings>();
			containerRegistry.GetContainer().RegisterType<States>(
				new ContainerControlledLifetimeManager(),
				new InjectionConstructor(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.states"))
			);

			// Singleton
			containerRegistry.RegisterSingleton<AlbumContainer>();
			containerRegistry.RegisterSingleton<MediaFactory>();
			containerRegistry.RegisterSingleton<ViewModelFactory>();
			containerRegistry.RegisterSingleton<ExternalToolsFactory>();
			containerRegistry.RegisterSingleton<PriorityTaskQueue>();
			containerRegistry.RegisterSingleton<AlbumHistoryManager>();
			containerRegistry.RegisterSingleton<MediaFileManager>();
			containerRegistry.RegisterSingleton<GeoCodingManager>();
			containerRegistry.RegisterSingleton<NotificationManager>();
			containerRegistry.RegisterSingleton<PluginManager>();
			containerRegistry.RegisterSingleton<AlbumViewerManager>();
			containerRegistry.RegisterSingleton<MainAlbumSelector>();
			containerRegistry.RegisterSingleton<EditorAlbumSelector>();

			// Map
			containerRegistry.Register<IMapControl, MapControl>();

			// マウス・キー操作受信
			containerRegistry.Register<IGestureReceiver, GestureReceiver>();

			// ダイアログ
			containerRegistry.RegisterDialogWindow<MediaBoxWindow>();
			containerRegistry.RegisterDialog<AlbumEditorWindow>();


			this._logging = this.Container.Resolve<ILogging>();
			this._logging.Log("ロガー取得");
			this._logging.Log($"起動時刻{launchTime:HH:mm:ss.fff}");


			// 設定読み込み
			this._settings = this.Container.Resolve<ISettings>();
			this._settings.Load();
			this._logging.Log("設定読み込み完了");

			// 状態読み込み
			this._states = this.Container.Resolve<States>();
			this._states.Load();
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
			containerRegistry.RegisterInstance(new DocumentDb("MediaBox.Files.db"));
			containerRegistry.RegisterInstance(dbContext);
			this._logging.Log("DB設定完了");
		}

		/// <summary>
		/// 終了時処理
		/// </summary>
		/// <param name="e">イベント引数</param>
		protected override void OnExit(ExitEventArgs e) {
			base.OnExit(e);
			this._settings.Save();
			this._states.Save();
			this._logging.Log("設定保存完了");

			this._mutex.Close();
			this._mutex.Dispose();
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
