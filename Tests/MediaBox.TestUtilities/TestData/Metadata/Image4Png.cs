using System.IO;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class Image4Png {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Image4Png));
			var test = new TestFile() {
				FileName = TestFileNames.Image4Png,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.Image4Png),
				Extension = ".png",
				CreationTime = fi.CreationTime,
				ModifiedTime = fi.LastWriteTime,
				LastAccessTime = fi.LastAccessTime,
				FileSize = 272,
				Resolution = new ComparableSize(11, 6),
				Location = null,
				Rate = 0,
				IsInvalid = false,
				Tags = new string[] { },
				Exists = true,
				Png = new Png {
					BitsPerSample = 8,
					ColorType = 2,
					CompressionType = 0,
					FilterMethod = 0,
					InterlaceMethod = 0
				}
			};
			return test;

		}
	}
}
