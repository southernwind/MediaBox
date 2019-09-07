using System.IO;

using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class NotExistsFileJpg {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.NotExistsFileJpg));
			var test = new TestFile {
				FileName = TestFileNames.NotExistsFileJpg,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.NotExistsFileJpg),
				Extension = ".jpg",
				Exists = false,
				Jpeg = new Jpeg()
			};

			return test;
		}
	}
}
