using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

using Microsoft.Data.Sqlite;

using Prism.Ioc;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.About;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Container;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Editor;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.History;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Loader;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Viewer;
using SandBeige.MediaBox.Composition.Interfaces.Models.Gesture;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.Notification;
using SandBeige.MediaBox.Composition.Interfaces.Models.Plugin;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue;
using SandBeige.MediaBox.Composition.Interfaces.Models.Tool;
using SandBeige.MediaBox.Composition.Interfaces.Services;
using SandBeige.MediaBox.Composition.Interfaces.Services.MediaFileServices;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Library.Services;
using SandBeige.MediaBox.Models.About;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Box;
using SandBeige.MediaBox.Models.Album.Editor;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.History;
using SandBeige.MediaBox.Models.Album.Loader;
using SandBeige.MediaBox.Models.Album.Selector;
using SandBeige.MediaBox.Models.Album.Sort;
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
using SandBeige.MediaBox.Services;
using SandBeige.MediaBox.Services.MediaFileServices;
using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.Views;
using SandBeige.MediaBox.Views.About;
using SandBeige.MediaBox.Views.Album.Box;
using SandBeige.MediaBox.Views.Album.Editor;
using SandBeige.MediaBox.Views.Album.Filter;
using SandBeige.MediaBox.Views.Album.Sort;
using SandBeige.MediaBox.Views.Dialog;
using SandBeige.MediaBox.Views.Map;
using SandBeige.MediaBox.Views.Media.ThumbnailCreator;
using SandBeige.MediaBox.Views.Settings;
using SandBeige.MediaBox.Views.Utils;


namespace SandBeige.MediaBox {
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App {
		private ISettings? _settings;
		private ILogging? _logging;
		private Mutex? _mutex;
		private IStates? _states;
		private Views.SplashScreen? _splashScreen;
		private string? _stateFilePath;
		private string? _settingsFilePath;

		/// <summary>
		/// 初期処理
		/// </summary>
		/// <param name="e"></param>
		protected override void OnStartup(StartupEventArgs e) {
			this._logging = new Logging();

			this._logging!.Log($"起動時刻{DateTime.Now:HH:mm:ss.fff}");

			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
			this._stateFilePath = Path.Combine(baseDirectory, "MediaBox.states");
			this._settingsFilePath = Path.Combine(baseDirectory, "MediaBox.settings");
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
			base.OnStartup(e);
		}

		/// <summary>
		/// DIコンテナ登録
		/// </summary>
		/// <param name="containerRegistry">コンテナ</param>
		protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry) {
			base.RegisterRequiredTypes(containerRegistry);
			// ロガー
			containerRegistry.RegisterSingleton<ILogging, Logging>();

			// 設定
			containerRegistry.Register<IGeneralSettings, GeneralSettings>();
			containerRegistry.Register<IPathSettings, PathSettings>();
			containerRegistry.Register<IScanSettings, ScanSettings>();
			containerRegistry.Register<IViewerSettings, ViewerSettings>();
			containerRegistry.Register<IPluginSettings, PluginSettings>();
			containerRegistry.RegisterSingleton<ISettings, Settings>();
			containerRegistry.Register<IAlbumStates, AlbumStates>();
			containerRegistry.Register<ISizeStates, SizeStates>();
			containerRegistry.RegisterSingleton<IStates, States>();

			// Singleton
			containerRegistry.RegisterSingleton<IAlbumContainer, AlbumContainer>();
			containerRegistry.RegisterSingleton<IMediaFactory, MediaFactory>();
			containerRegistry.RegisterSingleton<ViewModelFactory>();
			containerRegistry.RegisterSingleton<IExternalToolsFactory, ExternalToolsFactory>();
			containerRegistry.RegisterSingleton<IAlbumLoaderFactory, AlbumLoaderFactory>();
			containerRegistry.RegisterSingleton<IAlbumSelectorProvider, AlbumSelectorFactory>();
			containerRegistry.RegisterSingleton<IPriorityTaskQueue, PriorityTaskQueue>();
			containerRegistry.RegisterSingleton<IAlbumHistoryRegistry, AlbumHistoryManager>();
			containerRegistry.RegisterSingleton<IMediaFileManager, MediaFileManager>();
			containerRegistry.RegisterSingleton<IGeoCodingService, GeoCodingService>();
			containerRegistry.RegisterSingleton<INotificationManager, NotificationManager>();
			containerRegistry.RegisterSingleton<IPluginManager, PluginManager>();
			containerRegistry.RegisterSingleton<IAlbumViewerManager, AlbumViewerManager>();
			containerRegistry.RegisterSingleton<IFilterItemFactory, FilterItemFactory>();
			containerRegistry.RegisterSingleton<VolatilityStateShareService>();

			// Interface
			containerRegistry.Register<IAboutModel, AboutModel>();
			containerRegistry.Register<IAlbumSelector, AlbumSelector>();
			containerRegistry.Register<IFilterDescriptionManager, FilterDescriptionManager>();
			containerRegistry.Register<ISortDescriptionManager, SortDescriptionManager>();
			containerRegistry.Register<IAlbumModel, AlbumModel>();
			containerRegistry.Register<IAlbumObjectCreator, AlbumObjectCreator>();
			containerRegistry.Register<IAlbumBox, AlbumBox>();
			containerRegistry.Register<IAlbumEditor, AlbumEditor>();
			containerRegistry.Register<IAlbumForEditorModel, AlbumForEditorModel>();
			containerRegistry.Register<IAlbumBoxSelector, AlbumBoxSelector>();
			containerRegistry.Register<IGpsSelector, GpsSelector>();
			containerRegistry.Register<IMediaFileInformation, MediaFileInformation>();

			// Service
			containerRegistry.Register<IOpenFileDialogService, OpenFileDialogService>();
			containerRegistry.Register<IFolderSelectionDialogService, FolderSelectionDialogService>();
			containerRegistry.Register<IVideoThumbnailService, VideoThumbnailService>();
			containerRegistry.Register<IImageThumbnailService, ImageThumbnailService>();

			// Map
			containerRegistry.Register<IMapControl, MapControl>();

			// マウス・キー操作受信
			containerRegistry.Register<IGestureReceiver, GestureReceiver>();

			// ダイアログ
			containerRegistry.RegisterDialogWindow<MediaBoxWindow>();
			containerRegistry.RegisterDialog<AboutWindow>();
			containerRegistry.RegisterDialog<AlbumBoxSelectorWindow>();
			containerRegistry.RegisterDialog<AlbumEditorWindow>();
			containerRegistry.RegisterDialog<ColumnSettingsWindow>();
			containerRegistry.RegisterDialog<CommonDialogWindow>();
			containerRegistry.RegisterDialog<GpsSelectorWindow>();
			containerRegistry.RegisterDialog<RenameWindow>();
			containerRegistry.RegisterDialog<SettingsWindow>();
			containerRegistry.RegisterDialog<SetFilterWindow>();
			containerRegistry.RegisterDialog<SetSortWindow>();
			containerRegistry.RegisterDialog<ThumbnailCreatorWindow>();
		}

		/// <summary>
		/// DIコンテナ登録
		/// </summary>
		/// <param name="containerRegistry">コンテナ</param>
		protected override void RegisterTypes(IContainerRegistry containerRegistry) {
			// 設定読み込み
			this._settings = this.Container.Resolve<ISettings>();
			this._settings.SetFilePath(this._settingsFilePath!);
			this._settings.Load();
			this._logging!.Log("設定読み込み完了");

			// 状態読み込み
			this._states = this.Container.Resolve<IStates>();
			this._states.SetFilePath(this._stateFilePath!);
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
			containerRegistry.RegisterInstance<IDocumentDb>(new DocumentDb("MediaBox.Files.db"));
			containerRegistry.RegisterInstance<IMediaBoxDbContext>(dbContext);
			this._logging.Log("DB設定完了");
		}

		/// <summary>
		/// 初期ウィンドウ作成
		/// </summary>
		/// <returns></returns>
		protected override Window CreateShell() {
			return this.Container.Resolve<MainWindow>();
		}

		/// <summary>
		/// 初期処理終了後処理
		/// </summary>
		protected override void OnInitialized() {
			base.OnInitialized();
			this._logging!.Log("VM,メイン画面インスタンス作成完了");
			this._splashScreen!.Close();
			this.Container.Resolve<INotificationManager>().Notify(new Information(null, "起動完了"));

		}

		/// <summary>
		/// 終了時処理
		/// </summary>
		/// <param name="e">イベント引数</param>
		protected override void OnExit(ExitEventArgs e) {
			base.OnExit(e);
			this._settings!.Save();
			this._states!.Save();
			this._logging!.Log("設定保存完了");

			this._mutex!.Close();
			this._mutex.Dispose();
		}

		/// <summary>
		/// 集約エラーハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			if (e.ExceptionObject is Exception ex) {
				this._logging!.Log("集約エラーハンドラ", LogLevel.Warning, ex);
#if DEBUG
				// TODO:vs経由でデバッグ中に終了すると毎度例外が出てしまうので、応急処置
				if (ex.StackTrace!.Contains("Microsoft.VisualStudio.DesignTools.WpfTap.Networking.ProtocolHandler.HandleMessage")) {
					Environment.Exit(1);
				}
#endif
			} else {
				this._logging!.Log(e.ToString(), LogLevel.Warning);
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
