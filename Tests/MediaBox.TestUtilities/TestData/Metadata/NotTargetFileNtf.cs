using System.IO;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class NotTargetFileNtf {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.NotTargetFileNtf));
			var test = new TestFile {
				FileName = TestFileNames.NotTargetFileNtf,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.NotTargetFileNtf),
				Extension = ".ntf",
				CreationTime = fi.CreationTime,
				ModifiedTime = fi.LastWriteTime,
				LastAccessTime = fi.LastAccessTime,
				FileSize = 3667,
				Exists = true
			};

			return test;
		}
	}
}
