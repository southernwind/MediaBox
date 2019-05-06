using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;

using Microsoft.Data.Sqlite;

using NUnit.Framework;

using Reactive.Bindings;

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

using Unity;
using Unity.Lifetime;
namespace SandBeige.MediaBox.Tests.Models {
	internal class ModelTestClassBase : TestClassBase {
		protected Dictionary<string, string> TestDirectories;
		protected MediaFactory MediaFactory;
		protected ISettings Settings;
		protected States States;
		protected PriorityTaskQueue TaskQueue;
		protected MediaBoxDbContext DataBase;
		protected Logging Logging;

		[OneTimeSetUp]
		public override void OneTimeSetUp() {
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
			base.OneTimeSetUp();
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
			this.TaskQueue = Get.Instance<PriorityTaskQueue>();
			this.TaskQueue.TaskStart();
			this.Logging = Get.Instance<ILogging>() as Logging;
			ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
		}

		[TearDown]
		public override void TearDown() {
			if (this.DataBase != null) {
				lock (this.DataBase) {
					this.DataBase.Database.EnsureDeleted();
				}
			}
			if (this.TestDirectories != null) {
				// Directory
				foreach (var dir in this.TestDirectories) {
					DirectoryUtility.AllFileDelete(dir.Value);
				}
			}
			UnityConfig.UnityContainer.Dispose();
			base.TearDown();
		}

		/// <summary>
		/// ファイルシステム使用
		/// </summary>
		protected void UseFileSystem() {
			// テスト用ディレクトリ作成
			this.TestDirectories = new Dictionary<string, string> {
				{"0", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir0")},
				{"1", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir1")},
				{"sub", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"dir1\sub")},
				{"2", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir2")},
				{"3", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir3")},
				{"4", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir4")},
				{"5", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir5")},
				{"6", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir6")}
			};
			foreach (var dir in this.TestDirectories) {
				DirectoryUtility.AllFileDelete(dir.Value);
			}
			foreach (var dir in this.TestDirectories) {
				Directory.CreateDirectory(dir.Value);
			}

			// サムネイルディレクトリクリーン
			DirectoryUtility.AllFileDelete(this.Settings.PathSettings.ThumbnailDirectoryPath.Value);
			Directory.CreateDirectory(this.Settings.PathSettings.ThumbnailDirectoryPath.Value);
			foreach (var i in Enumerable.Range(0, 256)) {
				Directory.CreateDirectory(Path.Combine(this.Settings.PathSettings.ThumbnailDirectoryPath.Value, i.ToString("X2")));
			}

			// フィルタディレクトリクリーン
			DirectoryUtility.AllFileDelete(this.Settings.PathSettings.FilterDirectoryPath.Value);
			Directory.CreateDirectory(this.Settings.PathSettings.FilterDirectoryPath.Value);
		}

		/// <summary>
		/// インメモリデータベース使用
		/// </summary>
		protected void UseDataBaseInMemory() {
			this.DataBase = new MediaBoxDbContext(new SqliteConnection(":memory:"));
			UnityConfig.UnityContainer.RegisterInstance(this.DataBase, new ContainerControlledLifetimeManager());
			this.DataBase.Database.EnsureCreated();
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
