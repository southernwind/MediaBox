﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.TestData;
using SandBeige.MediaBox.Utilities;
namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileManagerTest : TestClassBase {
		[SetUp]
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();
			this.UseFileSystem();
			this.Settings.ScanSettings.ScanDirectories.Clear();
			this.Settings.ScanSettings.ScanDirectories
				.Add(new ScanDirectory(TestDirectories["0"], true, true));
		}

		[Test]
		public void ファイル初期読み込み() {
			FileUtility.Copy(TestDataDir, TestDirectories["0"], TestFileNames.Image1Jpg, TestFileNames.NoExifJpg);
			var mfm = Get.Instance<MediaFileManager>();
			var addedFiles = new List<IMediaFileModel>();
			var are = new AutoResetEvent(false);
			mfm.OnRegisteredMediaFiles.Subscribe(x => {
				addedFiles.AddRange(x);
				are.Set();
			});
			are.WaitOne();
			addedFiles.Count.Is(2);
			var tfs = new TestFiles(TestDirectories["0"]);
			addedFiles.Check(tfs.Image1Jpg, tfs.NoExifJpg);

			this.DataBase.MediaFiles.Count().Is(2);
			this.DataBase.MediaFiles.Check(tfs.Image1Jpg, tfs.NoExifJpg);
		}

		[Test]
		public void 初期化後に監視ディレクトリ追加() {
			FileUtility.Copy(TestDataDir, TestDirectories["1"], TestFileNames.Image1Jpg, TestFileNames.NoExifJpg);
			var mfm = Get.Instance<MediaFileManager>();
			this.Settings.ScanSettings.ScanDirectories
				.Add(new ScanDirectory(TestDirectories["1"], true, true));

			var addedFiles = new List<IMediaFileModel>();
			var are = new AutoResetEvent(false);
			mfm.OnRegisteredMediaFiles.Subscribe(x => {
				addedFiles.AddRange(x);
				are.Set();
			});
			are.WaitOne();
			addedFiles.Count.Is(2);
			var tfs = new TestFiles(TestDirectories["1"]);
			addedFiles.Check(tfs.Image1Jpg, tfs.NoExifJpg);

			this.DataBase.MediaFiles.Count().Is(2);
			this.DataBase.MediaFiles.Check(tfs.Image1Jpg, tfs.NoExifJpg);
		}

		[Test]
		public void ファイル作成検出() {
			var mfm = Get.Instance<MediaFileManager>();
			var addedFiles = new List<IMediaFileModel>();
			var are = new AutoResetEvent(false);
			mfm.OnRegisteredMediaFiles.Subscribe(x => {
				addedFiles.AddRange(x);
				are.Set();
			});
			FileUtility.Copy(TestDataDir, TestDirectories["0"], TestFileNames.Image1Jpg, TestFileNames.NoExifJpg);
			are.WaitOne();
			addedFiles.Count.Is(2);
			var tfs = new TestFiles(TestDirectories["0"]);
			addedFiles.Check(tfs.Image1Jpg, tfs.NoExifJpg);

			this.DataBase.MediaFiles.Count().Is(2);
			this.DataBase.MediaFiles.Check(tfs.Image1Jpg, tfs.NoExifJpg);
		}
	}
}
