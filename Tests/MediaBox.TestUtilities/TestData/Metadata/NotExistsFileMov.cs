using System.IO;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class NotExistsFileMov {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.NotExistsFileMov));
			var test = new TestFile {
				FileName = TestFileNames.NotExistsFileMov,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.NotExistsFileMov),
				Extension = ".mov",
				Exists = false
			};

			return test;
		}
	}
}
