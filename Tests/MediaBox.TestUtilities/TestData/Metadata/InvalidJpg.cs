using System.IO;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class InvalidJpg {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.InvalidJpg));
			var test = new TestFile() {
				FileName = TestFileNames.InvalidJpg,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.InvalidJpg),
				Extension = ".jpg",
				CreationTime = fi.CreationTime,
				ModifiedTime = fi.LastWriteTime,
				LastAccessTime = fi.LastAccessTime,
				FileSize = 701,
				Resolution = null,
				Location = null,
				Rate = 0,
				IsInvalid = true,
				Tags = new string[] { },
				Exists = true,
				Jpeg = null
			};
			return test;
		}
	}
}
