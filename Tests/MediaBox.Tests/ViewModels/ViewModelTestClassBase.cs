using System;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using Livet;

using Microsoft.Data.Sqlite;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Gesture;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.States;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Tests.Implements;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.TestData;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels;

using Unity;
using Unity.Lifetime;

namespace SandBeige.MediaBox.Tests.ViewModels {
	internal class ViewModelTestClassBase : TestClassBase {
		protected MediaFactory MediaFactory;
		protected ViewModelFactory ViewModelFactory;
		protected ISettings Settings;
		protected States States;
		protected MediaBoxDbContext DataBase;
		protected PriorityTaskQueue TaskQueue;

		[OneTimeSetUp]
		public override void OneTimeSetUp() {
			base.OneTimeSetUp();
			DispatcherHelper.UIDispatcher = Dispatcher.CurrentDispatcher;
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
		}

		[SetUp]
		public override void SetUp() {
			base.SetUp();
			TypeRegistrations.RegisterType(new UnityContainer());

			File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.settings"));
			File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.states"));
			UnityConfig.UnityContainer.RegisterType<ILogging, Logging>(new ContainerControlledLifetimeManager());
			UnityConfig.UnityContainer.RegisterType<IMapControl, MapControlForTest>();
			UnityConfig.UnityContainer.RegisterType<IGestureReceiver, GestureReceiverForTest>();
			this.Settings = Get.Instance<ISettings>();
			this.Settings.Load();
			this.Settings.ScanSettings.ScanDirectories.Clear();
			this.States = Get.Instance<States>();
			this.States.Load();
			this.UseDataBaseFile();
			this.MediaFactory = Get.Instance<MediaFactory>();
			this.ViewModelFactory = Get.Instance<ViewModelFactory>();
			this.TaskQueue = Get.Instance<PriorityTaskQueue>();

			ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
		}

		[TearDown]
		public override void TearDown() {
			if (this.DataBase != null) {
				lock (this.DataBase) {
					this.DataBase.Database.EnsureDeleted();
				}
			}
			UnityConfig.UnityContainer.Dispose();
			base.TearDown();
		}

		protected void UseReactiveScheduler() {
			ReactivePropertyScheduler.SetDefault(ReactivePropertyScheduler.Default);
		}

		/// <summary>
		/// データベースファイル使用
		/// </summary>
		protected void UseDataBaseFile() {
			// DataBase
			var sb = new SqliteConnectionStringBuilder {
				DataSource = this.Settings.PathSettings.DataBaseFilePath.Value
			};
			this.DataBase = new MediaBoxDbContext(new SqliteConnection(sb.ConnectionString));
			lock (this.DataBase) {
				UnityConfig.UnityContainer.RegisterInstance(this.DataBase, new ContainerControlledLifetimeManager());
				this.DataBase.Database.EnsureDeleted();
				this.DataBase.Database.EnsureCreated();
			}
		}

		/// <summary>
		/// タスク完了待機
		/// </summary>
		/// <param name="timeoutMilliSeconds">タイムアウトまでの秒数</param>
		/// <returns>タスク</returns>
		protected async Task WaitTaskCompleted(int timeoutMilliSeconds) {
			await
				this.TaskQueue
					.AllTaskCompleted
					.FirstAsync()
					.Timeout(TimeSpan.FromMilliseconds(timeoutMilliSeconds));
		}

		/// <summary>
		/// テスト用レコード登録
		/// </summary>
		/// <param name="testFile">テストファイル</param>
		/// <returns>登録したレコードとモデル</returns>
		protected (MediaFile, IMediaFileModel) Register(TestFile testFile) {
			var media = this.MediaFactory.Create(testFile.FilePath);

			var record = media.CreateDataBaseRecord();

			if (record.Latitude is { } lat && record.Longitude is { } lon) {
				if (!this.DataBase.Positions.Any(x => x.Latitude == lat && x.Longitude == lon)) {
					this.DataBase.Positions.Add(new Position() { Latitude = lat, Longitude = lon });
				}
			}
			this.DataBase.MediaFiles.Add(record);
			this.DataBase.SaveChanges();
			media.MediaFileId = record.MediaFileId;

			return (record, media);
		}
	}
}
