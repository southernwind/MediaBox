using System.IO;

namespace SandBeige.MediaBox.TestUtilities.TestData.Metadata {

	public static class SpecialFileNameVideoMov {
		public static TestFile Get(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.SpecialFileNameVideoMov));
			var test = Video1Mov.Get(baseDirectoryPath);
			test.FileName = TestFileNames.SpecialFileNameVideoMov;
			test.FilePath = Path.Combine(baseDirectoryPath, TestFileNames.SpecialFileNameVideoMov);
			test.Extension = ".mov";
			test.CreationTime = fi.CreationTime;
			test.ModifiedTime = fi.LastWriteTime;
			test.LastAccessTime = fi.LastAccessTime;

			return test;

		}
	}
}
