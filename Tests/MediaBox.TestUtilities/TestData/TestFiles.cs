using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.TestUtilities.TestData {

	/// <summary>
	/// テストファイル一覧
	/// </summary>
	public static class TestFileNames {
		public const string Image1Jpg = "image1.jpg";
		public const string Image2Jpg = "image2.jpg";
		public const string Image3Jpg = "image3.jpg";
		public const string Image4Png = "image4.png";
		public const string Image5Bmp = "image5.bmp";
		public const string Image6Gif = "image6.gif";
		public const string Video1Mov = "video1.mov";
		public const string NoExifJpg = "no_exif.jpg";
		public const string NotTargetFile = "not_target_file";
		public const string NotTargetFileNtf = "not_target_file.ntf";
		public const string NotExistsFileJpg = "not_exists_file.jpg";
		public const string NotExistsFileMov = "not_exists_file.mov";
		public const string InvalidJpg = "invalid.jpg";
		public const string SpecialFileNameImageJpg = "image(e.g,;@^ -~=$#!'`[]{}_%&).jpg";
		public const string SpecialFileNameVideoMov = "video(e.g,;@^ -~=$#!'`[]{}_%&).mov";
	}

	/// <summary>
	/// テストファイル保持クラス
	/// </summary>
	public class TestFiles : IEnumerable<TestFile> {
		public readonly TestFile Image1Jpg;
		public readonly TestFile Image2Jpg;
		public readonly TestFile Image3Jpg;
		public readonly TestFile Image4Png;
		public readonly TestFile Image5Bmp;
		public readonly TestFile Image6Gif;
		public readonly TestFile Video1Mov;
		public readonly TestFile NoExifJpg;
		public readonly TestFile InvalidJpg;
		public readonly TestFile SpecialFileNameImageJpg;
		public readonly TestFile SpecialFileNameVideoMov;
		public readonly List<TestFile> ImageFiles = new List<TestFile>();
		public readonly List<TestFile> VideoFiles = new List<TestFile>();

		public IEnumerator<TestFile> GetEnumerator() {
			return this.ImageFiles.Union(this.VideoFiles).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="baseDirectoryPath">テストファイルのディレクトリパス</param>
		public TestFiles(string baseDirectoryPath) {
			this.Image1Jpg = Metadata.Image1Jpg.Get(baseDirectoryPath);
			this.Image2Jpg = Metadata.Image2Jpg.Get(baseDirectoryPath);
			this.Image3Jpg = Metadata.Image3Jpg.Get(baseDirectoryPath);
			this.Image4Png = Metadata.Image4Png.Get(baseDirectoryPath);
			this.Image5Bmp = Metadata.Image5Bmp.Get(baseDirectoryPath);
			this.Image6Gif = Metadata.Image6Gif.Get(baseDirectoryPath);
			this.NoExifJpg = Metadata.NoExifJpg.Get(baseDirectoryPath);
			this.Video1Mov = Metadata.Video1Mov.Get(baseDirectoryPath);
			this.InvalidJpg = Metadata.InvalidJpg.Get(baseDirectoryPath);
			this.SpecialFileNameImageJpg = Metadata.SpecialFileNameImageJpg.Get(baseDirectoryPath);
			this.SpecialFileNameVideoMov = Metadata.SpecialFileNameVideoMov.Get(baseDirectoryPath);
			this.ImageFiles.AddRange(
				this.Image1Jpg,
				this.Image2Jpg,
				this.Image3Jpg,
				this.Image4Png,
				this.Image5Bmp,
				this.Image6Gif,
				this.NoExifJpg,
				this.InvalidJpg,
				this.SpecialFileNameImageJpg
			);
			this.VideoFiles.AddRange(
				this.Video1Mov,
				this.SpecialFileNameVideoMov
			);
		}
	}
}
