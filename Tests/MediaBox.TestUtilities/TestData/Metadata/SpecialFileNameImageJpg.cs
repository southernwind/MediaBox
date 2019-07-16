using System.IO;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class SpecialFileNameImageJpg {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.SpecialFileNameImageJpg));
			var test = Image3Jpg.Get(baseDirectoryPath);
			test.FileName = TestFileNames.SpecialFileNameImageJpg;
			test.FilePath = Path.Combine(baseDirectoryPath, TestFileNames.SpecialFileNameImageJpg);
			test.Extension = ".jpg";
			test.CreationTime = fi.CreationTime;
			test.ModifiedTime = fi.LastWriteTime;
			test.LastAccessTime = fi.LastAccessTime;
			return test;
		}
	}
}
