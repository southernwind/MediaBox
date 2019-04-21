using System.IO;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.TestUtilities.TestData {

	/// <summary>
	/// テストファイル一覧
	/// </summary>
	public static class TestFileNames {
		public static string Image1Jpg = "image1.jpg";
		public static string Image2Jpg = "image2.jpg";
		public static string Image3Jpg = "image3.jpg";
		public static string Image4Jpg = "image4.jpg";
		public static string Image5Jpg = "image5.jpg";
		public static string Image6Jpg = "image6.jpg";
		public static string Image7Jpg = "image7.jpg";
		public static string Image8Jpg = "image8.jpg";
		public static string Image9Png = "image9.png";
		public static string NoExifJpg = "no_exif.jpg";
		public static string NotTargetFile = "not_target_file";
		public static string NotTargetFileNtf = "not_target_file.ntf";
	}

	/// <summary>
	/// テストファイル保持クラス
	/// </summary>
	public class TestFiles {
		public readonly TestFile Image1Jpg;
		public readonly TestFile Image2Jpg;
		public readonly TestFile Image3Jpg;
		public readonly TestFile Image4Jpg;
		public readonly TestFile Image5Jpg;
		public readonly TestFile Image6Jpg;
		public readonly TestFile Image7Jpg;
		public readonly TestFile Image8Jpg;
		public readonly TestFile Image9Png;
		public readonly TestFile NoExifJpg;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="baseDirectoryPath">テストファイルのディレクトリパス</param>
		public TestFiles(string baseDirectoryPath) {
			var fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Image1Jpg));
			this.Image1Jpg = new TestFile() {
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
				Exists = true
			};

			fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Image2Jpg));
			this.Image2Jpg = new TestFile() {
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
				Exists = true
			};

			fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Image3Jpg));
			this.Image3Jpg = new TestFile() {
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
				Exists = true
			};

			fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.NoExifJpg));
			this.NoExifJpg = new TestFile() {
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
				Exists = true
			};
		}
	}
}
