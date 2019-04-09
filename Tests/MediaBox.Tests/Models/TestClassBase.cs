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
namespace SandBeige.MediaBox.Tests.Models {
	internal class TestClassBase {
		protected static string TestDataDir;
		protected static Dictionary<string, string> TestDirectories;
		protected MediaFactory MediaFactory;
		protected ISettings Settings;

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
			var dbContext = new MediaBoxDbContext(new SqliteConnection(sb.ConnectionString));
			UnityConfig.UnityContainer.RegisterInstance(dbContext, new ContainerControlledLifetimeManager());
			dbContext.Database.EnsureDeleted();
			dbContext.Database.EnsureCreated();
		}


	}


	internal class TestFiles {
		public static string Image1Jpg;
		public static string Image2Jpg;
		public static string Image3Jpg;
		public static string Image4Jpg;
		public static string Image5Jpg;
		public static string Image6Jpg;
		public static string Image7Jpg;
		public static string Image8Jpg;
		public static string Image9Png;

		public TestFiles(string baseDirectoryPath) {
			Image1Jpg = Path.Combine(baseDirectoryPath, "image1.jpg");
			Image2Jpg = Path.Combine(baseDirectoryPath, "image2.jpg");
			Image3Jpg = Path.Combine(baseDirectoryPath, "image3.jpg");
			Image4Jpg = Path.Combine(baseDirectoryPath, "image4.jpg");
			Image5Jpg = Path.Combine(baseDirectoryPath, "image5.jpg");
			Image6Jpg = Path.Combine(baseDirectoryPath, "image6.jpg");
			Image7Jpg = Path.Combine(baseDirectoryPath, "image7.jpg");
			Image8Jpg = Path.Combine(baseDirectoryPath, "image8.jpg");
			Image9Png = Path.Combine(baseDirectoryPath, "image9.png");
		}
	}
}
