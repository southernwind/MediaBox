using System;
using System.Collections.Generic;

using Microsoft.Data.Sqlite;

using Moq;

using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.TestUtilities {
	public class DbContextMockCreator : IDisposable {
		public Mock<MediaBoxDbContext> Mock {
			get;
		}

		public DbContextMockCreator() {
			var conn = new SqliteConnection("DataSource=:memory:");
			conn.Open();
			this.Mock = new Mock<MediaBoxDbContext>(() => new MediaBoxDbContext(conn));
			this.Mock.CallBase = true;
			this.Mock.Object.Database.EnsureCreated();
		}

		public void SetData<T>(params T[] data) where T : class {
			this.SetData(data);
		}

		public void SetData<T>(IEnumerable<T> data) where T : class {
			switch (data) {
				case IEnumerable<Album> albums:
					this.Mock.Object.Albums.AddRange(albums);
					break;
				case IEnumerable<AlbumBox> albumBoxes:
					this.Mock.Object.AlbumBoxes.AddRange(albumBoxes);
					break;
				case IEnumerable<AlbumMediaFile> albumMediaFiles:
					this.Mock.Object.AlbumMediaFiles.AddRange(albumMediaFiles);
					break;
				case IEnumerable<AlbumScanDirectory> albumScanDirectories:
					this.Mock.Object.AlbumScanDirectories.AddRange(albumScanDirectories);
					break;
			}
			this.Mock.Object.SaveChanges();
		}

		public void Dispose() {
			this.Mock.Object.Dispose();
		}
	}
}
