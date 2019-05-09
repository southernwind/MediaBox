using System.IO;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class NoExifJpg {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.NoExifJpg));
			var test = new TestFile() {
				FileName = TestFileNames.NoExifJpg,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.NoExifJpg),
				Extension = ".jpg",
				CreationTime = fi.CreationTime,
				ModifiedTime = fi.LastWriteTime,
				LastAccessTime = fi.LastAccessTime,
				FileSize = 771,
				Resolution = new ComparableSize(8, 12),
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
