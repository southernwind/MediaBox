using System.IO;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class Image3Jpg {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Image3Jpg));
			var test = new TestFile() {
				FileName = TestFileNames.Image3Jpg,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.Image3Jpg),
				Extension = ".jpg",
				CreationTime = fi.CreationTime,
				ModifiedTime = fi.LastWriteTime,
				LastAccessTime = fi.LastAccessTime,
				FileSize = 1818,
				Resolution = new ComparableSize(4, 4),
				Location = null,
				Rate = 0,
				IsInvalid = false,
				Tags = new string[] { },
				Exists = true,
				Jpeg = new Jpeg()
			};
			return test;
		}
	}
}
