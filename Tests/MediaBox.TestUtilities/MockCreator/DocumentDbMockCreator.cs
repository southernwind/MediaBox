
using Microsoft.Data.Sqlite;

using Moq;

using SandBeige.MediaBox.DataBase;

namespace SandBeige.MediaBox.TestUtilities.MockCreator {
	public class DocumentDbMockCreator {
		public Mock<DocumentDb> Mock {
			get;
		}

		public DocumentDbMockCreator() {
			var conn = new SqliteConnection("DataSource=:memory:");
			conn.Open();
			this.Mock = new Mock<DocumentDb>(() => new DocumentDb(":memory:")) {
				CallBase = true
			};
		}
	}
}
