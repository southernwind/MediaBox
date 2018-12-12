﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Livet;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Reactive.Bindings;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.TestUtilities;
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

			var settings = Get.Instance<ISettings>();
			// DataBase
			var scsb = new SqliteConnectionStringBuilder {
				DataSource = settings.PathSettings.DataBaseFilePath.Value
			};
			var dbContext = new MediaBoxDbContext(new SqliteConnection(scsb.ConnectionString));
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