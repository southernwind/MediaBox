using Microsoft.Data.Sqlite;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Settings;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Utilities;

using Unity;
using Unity.Lifetime;

namespace SandBeige.MediaBox.Tests.Utilities {
	[TestFixture]
	internal class GetTest {
		[Test]
		public void Instance() {
			TypeRegistrations.RegisterType(new UnityContainer());
			var settings = Get.Instance<ISettings>();
			// DataBase
			var sb = new SqliteConnectionStringBuilder {
				DataSource = settings.PathSettings.DataBaseFilePath.Value
			};
			var dbContext = new MediaBoxDbContext(new SqliteConnection(sb.ConnectionString));
			UnityConfig.UnityContainer.RegisterInstance(dbContext, new ContainerControlledLifetimeManager());
			dbContext.Database.EnsureDeleted();
			dbContext.Database.EnsureCreated();
			settings.IsInstanceOf<Settings>();
			Get.Instance<MediaFile>(@"C:\file.png").FileName.Value.Is("file.png");
		}
	}
}
