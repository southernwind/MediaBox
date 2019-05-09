using System.IO;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class Image2Jpg {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Image2Jpg));
			var test = new TestFile() {
				FileName = TestFileNames.Image2Jpg,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.Image2Jpg),
				Extension = ".jpg",
				CreationTime = fi.CreationTime,
				ModifiedTime = fi.LastWriteTime,
				LastAccessTime = fi.LastAccessTime,
				FileSize = 3682,
				Resolution = new ComparableSize(5, 5),
				Location = new GpsLocation(-35.184364, -132.183486, -20.311688),
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
