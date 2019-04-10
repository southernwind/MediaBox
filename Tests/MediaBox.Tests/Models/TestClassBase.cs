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
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Tests.Implements;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;

using Unity;
using Unity.Lifetime;
namespace SandBeige.MediaBox.Tests.Models {
	internal class TestClassBase {
		protected static string TestDataDir;
		protected static Dictionary<string, string> TestDirectories;
		protected MediaFactory MediaFactory;
		protected ISettings Settings;
		protected PriorityTaskQueue TaskQueue;
		protected MediaBoxDbContext DataBase;

		[OneTimeSetUp]
		public virtual void OneTimeSetUp() {
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
		}

		[SetUp]
		public virtual void SetUp() {
			TypeRegistrations.RegisterType(new UnityContainer());

			UnityConfig.UnityContainer.RegisterType<IMapControl, MapControlForTest>();
			this.Settings = Get.Instance<ISettings>();
			this.Settings.Load();
			this.MediaFactory = Get.Instance<MediaFactory>();
			this.TaskQueue = Get.Instance<PriorityTaskQueue>();
			this.TaskQueue.TaskStart();
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

		/// <summary>
		/// ファイルシステム使用
		/// </summary>
		protected void UseFileSystem() {
			// テスト用ディレクトリ作成
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
			var dbContext = new MediaBoxDbContext(new SqliteConnection(":memory:"));
			UnityConfig.UnityContainer.RegisterInstance(dbContext, new ContainerControlledLifetimeManager());
			dbContext.Database.EnsureDeleted();
			dbContext.Database.EnsureCreated();
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

	internal static class TestFileNames {
		public static string Image1Jpg = "image1.jpg";
		public static string Image2Jpg = "image2.jpg";
		public static string Image3Jpg = "image3.jpg";
		public static string Image4Jpg = "image4.jpg";
		public static string Image5Jpg = "image5.jpg";
		public static string Image6Jpg = "image6.jpg";
		public static string Image7Jpg = "image7.jpg";
		public static string Image8Jpg = "image8.jpg";
		public static string Image9Png = "image9.png";
	}

	internal class TestFiles {
		public string Image1Jpg;
		public string Image2Jpg;
		public string Image3Jpg;
		public string Image4Jpg;
		public string Image5Jpg;
		public string Image6Jpg;
		public string Image7Jpg;
		public string Image8Jpg;
		public string Image9Png;

		public TestFiles(string baseDirectoryPath) {
			this.Image1Jpg = Path.Combine(baseDirectoryPath, TestFileNames.Image1Jpg);
			this.Image2Jpg = Path.Combine(baseDirectoryPath, TestFileNames.Image2Jpg);
			this.Image3Jpg = Path.Combine(baseDirectoryPath, TestFileNames.Image3Jpg);
			this.Image4Jpg = Path.Combine(baseDirectoryPath, TestFileNames.Image4Jpg);
			this.Image5Jpg = Path.Combine(baseDirectoryPath, TestFileNames.Image5Jpg);
			this.Image6Jpg = Path.Combine(baseDirectoryPath, TestFileNames.Image6Jpg);
			this.Image7Jpg = Path.Combine(baseDirectoryPath, TestFileNames.Image7Jpg);
			this.Image8Jpg = Path.Combine(baseDirectoryPath, TestFileNames.Image8Jpg);
			this.Image9Png = Path.Combine(baseDirectoryPath, TestFileNames.Image9Png);
		}
	}
}
