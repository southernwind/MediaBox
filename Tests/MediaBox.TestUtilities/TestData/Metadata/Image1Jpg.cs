using System.IO;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class Image1Jpg {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Image1Jpg));
			var test = new TestFile() {
				FileName = TestFileNames.Image1Jpg,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.Image1Jpg),
				Extension = ".jpg",
				CreationTime = fi.CreationTime,
				ModifiedTime = fi.LastWriteTime,
				LastAccessTime = fi.LastAccessTime,
				FileSize = 3667,
				Resolution = new ComparableSize(7, 5),
				Location = new GpsLocation(34.697419, 135.533553, 20.65577),
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
