using System.IO;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class Video1Mov {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Video1Mov));
			var test = new TestFile() {
				FileName = TestFileNames.Video1Mov,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.Video1Mov),
				Extension = ".mov",
				CreationTime = fi.CreationTime,
				ModifiedTime = fi.LastWriteTime,
				LastAccessTime = fi.LastAccessTime,
				FileSize = 12117,
				Resolution = new ComparableSize(720, 1280),
				Location = new GpsLocation(35.6851, 139.6916, 161.155),
				Rate = 0,
				IsInvalid = false,
				Tags = new string[] { },
				Exists = true
			};

			return test;

		}
	}
}
