using System.Threading;
using System.Windows.Threading;

using Livet;

using Microsoft.Data.Sqlite;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.States;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Tests.Implements;
using SandBeige.MediaBox.TestUtilities;
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

			UnityConfig.UnityContainer.RegisterType<ILogging, Logging>(new ContainerControlledLifetimeManager());
			UnityConfig.UnityContainer.RegisterType<IMapControl, MapControlForTest>();
			this.Settings = Get.Instance<ISettings>();
			this.States = Get.Instance<States>();
			this.Settings.Load();
			this.Settings.ScanSettings.ScanDirectories.Clear();
			this.UseDataBaseFile();
			this.MediaFactory = Get.Instance<MediaFactory>();
			this.ViewModelFactory = Get.Instance<ViewModelFactory>();
			this.TaskQueue = Get.Instance<PriorityTaskQueue>();
			this.TaskQueue.TaskStart();
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

		/// <summary>
		/// データベースファイル使用
		/// </summary>
		protected void UseDataBaseFile() {
			// DataBase
			var sb = new SqliteConnectionStringBuilder {
				DataSource = this.Settings.PathSettings.DataBaseFilePath.Value
			};
			this.DataBase = new MediaBoxDbContext(new SqliteConnection(sb.ConnectionString));
			UnityConfig.UnityContainer.RegisterInstance(this.DataBase, new ContainerControlledLifetimeManager());
			this.DataBase.Database.EnsureDeleted();
			this.DataBase.Database.EnsureCreated();
		}
	}
}
