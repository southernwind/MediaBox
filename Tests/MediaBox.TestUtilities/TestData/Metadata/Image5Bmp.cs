using System.IO;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class Image5Bmp {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Image5Bmp));
			var test = new TestFile() {
				FileName = TestFileNames.Image5Bmp,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.Image5Bmp),
				Extension = ".bmp",
				CreationTime = fi.CreationTime,
				ModifiedTime = fi.LastWriteTime,
				LastAccessTime = fi.LastAccessTime,
				FileSize = 134,
				Resolution = new ComparableSize(5, 5),
				Location = null,
				Rate = 0,
				IsInvalid = false,
				Tags = new string[] { },
				Exists = true,
				Bmp = new Bmp() {
					BitsPerPixel = 24,
					Compression = 0,
					XPixelsPerMeter = 0,
					YPixelsPerMeter = 0,
					PaletteColorCount = 0,
					ImportantColorCount = 0
				}
			};
			return test;
		}
	}
}
