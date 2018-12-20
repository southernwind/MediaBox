using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Livet;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Tests.Implements;
using SandBeige.MediaBox.Utilities;
using Unity;
using Unity.Lifetime;
using UnityContainer = Unity.UnityContainer;

namespace SandBeige.MediaBox.Tests.ViewModels {
	internal class ViewModelTestClassBase {
		private static string _testDataDir;
		protected static string TestDirectory;

		[OneTimeSetUp]
		public virtual void OneTimeSetUp() {
			_testDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\");
			TestDirectory = _testDataDir;
			DispatcherHelper.UIDispatcher = Dispatcher.CurrentDispatcher;
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
		}

		[SetUp]
		public virtual void SetUp() {
			TypeRegistrations.RegisterType(new UnityContainer());

			UnityConfig.UnityContainer.RegisterType<IMapControl, MapControlForTest>();
			var settings = Get.Instance<ISettings>();
			// DataBase
			var sb = new SqliteConnectionStringBuilder {
				DataSource = settings.PathSettings.DataBaseFilePath.Value
			};
			var dbContext = new MediaBoxDbContext(new SqliteConnection(sb.ConnectionString));
			UnityConfig.UnityContainer.RegisterInstance(dbContext, new ContainerControlledLifetimeManager());
			dbContext.Database.EnsureDeleted();
			dbContext.Database.EnsureCreated();

		}

		[TearDown]
		public virtual void TearDown() {
			var db = Get.Instance<MediaBoxDbContext>();
			db.Database.EnsureDeleted();
			UnityConfig.UnityContainer.Dispose();
		}

	}
}
