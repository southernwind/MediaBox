using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Microsoft.Data.Sqlite;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Tests.Implements;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;

using Unity;
using Unity.Lifetime;

using UnityContainer = Unity.UnityContainer;

namespace SandBeige.MediaBox.Tests.Models {
	internal class TestClassBase {
		protected static string TestDataDir;
		protected static Dictionary<string, string> TestDirectories;
		protected MediaFactory MediaFactory;
		protected ISettings Settings;
		[OneTimeSetUp]
		public virtual void OneTimeSetUp() {
			TestDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\");
			TestDirectories = new Dictionary<string, string> {
				{"0", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir0")},
				{"1", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir1")},
				{"sub", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"dir1\sub")},
				{"2", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir2")},
				{"3", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir3")},
				{"4", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir4")},
				{"5", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir5")},
				{"6", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir6")}
			};
			foreach (var dir in TestDirectories) {
				DirectoryUtility.AllFileDelete(dir.Value);
			}
			foreach (var dir in TestDirectories) {
				Directory.CreateDirectory(dir.Value);
			}
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
		}

		[SetUp]
		public virtual void SetUp() {
			TypeRegistrations.RegisterType(new UnityContainer());

			UnityConfig.UnityContainer.RegisterType<IMapControl, MapControlForTest>();
			this.Settings = Get.Instance<ISettings>();
			this.Settings.Load();
			// DataBase
			var sb = new SqliteConnectionStringBuilder {
				DataSource = this.Settings.PathSettings.DataBaseFilePath.Value
			};
			var dbContext = new MediaBoxDbContext(new SqliteConnection(sb.ConnectionString));
			UnityConfig.UnityContainer.RegisterInstance(dbContext, new ContainerControlledLifetimeManager());
			dbContext.Database.EnsureDeleted();
			dbContext.Database.EnsureCreated();

			// サムネイルディレクトリ
			DirectoryUtility.AllFileDelete(this.Settings.PathSettings.ThumbnailDirectoryPath.Value);
			Directory.CreateDirectory(this.Settings.PathSettings.ThumbnailDirectoryPath.Value);
			foreach (var i in Enumerable.Range(0, 256)) {
				Directory.CreateDirectory(Path.Combine(this.Settings.PathSettings.ThumbnailDirectoryPath.Value, i.ToString("X2")));
			}

			this.MediaFactory = Get.Instance<MediaFactory>();

			this.Settings.ForTestSettings.RunOnBackground.Value = false;
		}

		[TearDown]
		public virtual void TearDown() {
			var db = Get.Instance<MediaBoxDbContext>();
			db.Database.EnsureDeleted();
			// Directory
			foreach (var dir in TestDirectories) {
				DirectoryUtility.AllFileDelete(dir.Value);
			}
			UnityConfig.UnityContainer.Dispose();
		}

	}
}
