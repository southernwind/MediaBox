using System.IO;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.TestUtilities.TestData {

	/// <summary>
	/// テストファイル一覧
	/// </summary>
	public static class TestFileNames {
		public const string Image1Jpg = "image1.jpg";
		public const string Image2Jpg = "image2.jpg";
		public const string Image3Jpg = "image3.jpg";
		public const string Image4Png = "image4.png";
		public const string Video1Mov = "video1.mov";
		public const string NoExifJpg = "no_exif.jpg";
		public const string NotTargetFile = "not_target_file";
		public const string NotTargetFileNtf = "not_target_file.ntf";
		public const string NotExistsFileJpg = "not_exists_file.jpg";
		public const string NotExistsFileMov = "not_exists_file.mov";
	}

	/// <summary>
	/// テストファイル保持クラス
	/// </summary>
	public class TestFiles {
		public readonly TestFile Image1Jpg;
		public readonly TestFile Image2Jpg;
		public readonly TestFile Image3Jpg;
		public readonly TestFile Image4Png;
		public readonly TestFile Video1Mov;
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

			fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Video1Mov));
			this.Video1Mov = new TestFile() {
				FileName = TestFileNames.Video1Mov,
				FilePath = Path.Combine(baseDirectoryPath, TestFileNames.Video1Mov),
				Extension = ".mov",
				CreationTime = fi.CreationTime,
				ModifiedTime = fi.LastWriteTime,
				LastAccessTime = fi.LastAccessTime,
				FileSize = 13967,
				Resolution = new ComparableSize(720, 1280),
				Location = new GpsLocation(35.6851, 139.7506, 30.012),
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

			fi = new FileInfo(Path.Combine(baseDirectoryPath, TestFileNames.Image4Png));
			this.Image4Png = new TestFile() {
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
				Exists = true
			};
		}
	}
}
