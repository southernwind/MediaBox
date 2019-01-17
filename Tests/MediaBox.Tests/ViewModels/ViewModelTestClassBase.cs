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
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Tests.Implements;
using SandBeige.MediaBox.Utilities;

using Unity;
using Unity.Lifetime;

using UnityContainer = Unity.UnityContainer;

namespace SandBeige.MediaBox.Tests.ViewModels {
	internal class ViewModelTestClassBase {
		protected static string TestDataDir;
		protected MediaFactory MediaFactory;

		[OneTimeSetUp]
		public virtual void OneTimeSetUp() {
			TestDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\");
			DispatcherHelper.UIDispatcher = Dispatcher.CurrentDispatcher;
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
		}

		[SetUp]
		public virtual void SetUp() {
			TypeRegistrations.RegisterType(new UnityContainer());

			UnityConfig.UnityContainer.RegisterType<IMapControl, MapControlForTest>();
			var settings = Get.Instance<ISettings>();
			settings.Load();
			// DataBase
			var sb = new SqliteConnectionStringBuilder {
				DataSource = settings.PathSettings.DataBaseFilePath.Value
			};
			var dbContext = new MediaBoxDbContext(new SqliteConnection(sb.ConnectionString));
			UnityConfig.UnityContainer.RegisterInstance(dbContext, new ContainerControlledLifetimeManager());
			dbContext.Database.EnsureDeleted();
			dbContext.Database.EnsureCreated();
			this.MediaFactory = Get.Instance<MediaFactory>();

			settings.ForTestSettings.RunOnBackground.Value = false;
		}

		[TearDown]
		public virtual void TearDown() {
			var db = Get.Instance<MediaBoxDbContext>();
			db.Database.EnsureDeleted();
			UnityConfig.UnityContainer.Dispose();
		}

	}
}
