using System;
using System.Collections.Generic;
using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;
namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileManagerTest : TestClassBase {

		[Test]
		public void ファイル初期読み込み() {
			this.UseDataBaseFile();
			this.UseFileSystem();
			this.Settings.ScanSettings.ScanDirectories.Clear();
			this.Settings.ScanSettings.ScanDirectories
				.Add(new ScanDirectory(TestDirectories["0"], true, true));

			FileUtility.Copy(TestDataDir, TestDirectories["0"], TestFileNames.Image1Jpg, TestFileNames.Image2Jpg);

			var mfm = Get.Instance<MediaFileManager>();
			var addedFiles = new List<IMediaFileModel>();
			var are = new AutoResetEvent(false);
			mfm.OnRegisteredMediaFiles.Subscribe(x => {
				addedFiles.AddRange(x);
				are.Set();
			});
			are.WaitOne();
			addedFiles.Count.Is(2);
		}
	}
}
