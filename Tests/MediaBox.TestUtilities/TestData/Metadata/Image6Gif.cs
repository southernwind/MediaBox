using System.IO;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class Image6Gif {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Image6Gif));
			var test = new TestFile() {
				FileName = TestFileNames.Image6Gif,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.Image6Gif),
				Extension = ".gif",
				CreationTime = fi.CreationTime,
				ModifiedTime = fi.LastWriteTime,
				LastAccessTime = fi.LastAccessTime,
				FileSize = 93,
				Resolution = new ComparableSize(7, 2),
				Location = null,
				Rate = 0,
				IsInvalid = false,
				Tags = new string[] { },
				Exists = true,
				Gif = new Gif {
					ColorTableSize = 16,
					IsColorTableSorted = 0,
					BitsPerPixel = 4,
					HasGlobalColorTable = 1,
					BackgroundColorIndex = 0,
					PixelAspectRatio = null
				}
			};

			return test;
		}
	}
}
