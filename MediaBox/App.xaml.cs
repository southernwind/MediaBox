﻿using System;
using System.IO;
using System.Linq;
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

using Unosquare.FFME;
using Unosquare.FFME.Shared;

namespace SandBeige.MediaBox {
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App {
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

			// 状態読み込み
			var states = Get.Instance<States>();
			states.Load();

			// MediaElement設定
			MediaElement.FFmpegDirectory = this._settings.PathSettings.FFmpegDirectoryPath.Value;
			MediaElement.FFmpegLoadModeFlags = FFmpegLoadMode.FullFeatures;
			// trueにしていると画面切り替え時にフリーズする。多分↓の問題なので2.80になったら直るっぽい？
			// TODO : https://github.com/unosquare/ffmediaelement/issues/287 が直ったらtrueにする。
			MediaElement.EnableWpfMultiThreadedVideo = false;

			// ディレクトリがなければ作成
			if (!Directory.Exists(this._settings.PathSettings.ThumbnailDirectoryPath.Value)) {
				Directory.CreateDirectory(this._settings.PathSettings.ThumbnailDirectoryPath.Value);
				foreach (var i in Enumerable.Range(0, 256)) {
					Directory.CreateDirectory(Path.Combine(this._settings.PathSettings.ThumbnailDirectoryPath.Value, i.ToString("X2")));
				}
			}

			// DataBase
			var sb = new SqliteConnectionStringBuilder {
				DataSource = this._settings.PathSettings.DataBaseFilePath.Value
			};
			var dbContext = new MediaBoxDbContext(new SqliteConnection(sb.ConnectionString));
			dbContext.Database.EnsureCreated();
			UnityConfig.UnityContainer.RegisterInstance(dbContext, new ContainerControlledLifetimeManager());

			// 画面起動
			this.MainWindow = new Views.MainWindow {
				DataContext = Get.Instance<MainWindowViewModel>()
			};

			this.MainWindow.ShowDialog();

			this._settings.Save();
			states.Save();
		}

		/// <summary>
		/// 集約エラーハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			if (e.ExceptionObject is Exception ex) {
				this._logging.Log("集約エラーハンドラ", LogLevel.Warning, ex);
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
