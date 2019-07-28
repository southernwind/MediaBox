using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.TestData;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileManagerTest : ModelTestClassBase {
		[SetUp]
		public override void SetUp() {
			base.SetUp();
			this.UseFileSystem();
			this.Settings.ScanSettings.ScanDirectories.Clear();
			this.Settings.ScanSettings.ScanDirectories
				.Add(new ScanDirectory(this.TestDirectories["0"], true, true));
		}

		[Test]
		public void ファイル初期読み込み() {
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["0"], TestFileNames.Image1Jpg, TestFileNames.NoExifJpg);
			using var mfm = Get.Instance<MediaFileManager>();
			var addedFiles = new List<IMediaFileModel>();
			using var are = new AutoResetEvent(false);
			mfm.OnRegisteredMediaFiles.Subscribe(x => {
				addedFiles.AddRange(x);
				are.Set();
			});
			are.WaitOne();
			addedFiles.Count.Is(2);
			var tfs = new TestFiles(this.TestDirectories["0"]);
			addedFiles.Check(tfs.Image1Jpg, tfs.NoExifJpg);

			lock (this.DataBase) {
				this.DataBase.MediaFiles.Count().Is(2);
				this.DataBase.MediaFiles.Check(tfs.Image1Jpg, tfs.NoExifJpg);
			}
		}

		[Test]
		public void 初期化後に監視ディレクトリ追加() {
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["1"], TestFileNames.Image1Jpg, TestFileNames.NoExifJpg);
			using var mfm = Get.Instance<MediaFileManager>();
			this.Settings.ScanSettings.ScanDirectories
				.Add(new ScanDirectory(this.TestDirectories["1"], true, true));

			var addedFiles = new List<IMediaFileModel>();
			using var are = new AutoResetEvent(false);
			mfm.OnRegisteredMediaFiles.Subscribe(x => {
				addedFiles.AddRange(x);
				are.Set();
			});
			are.WaitOne();
			addedFiles.Count.Is(2);
			var tfs = new TestFiles(this.TestDirectories["1"]);
			addedFiles.Check(tfs.Image1Jpg, tfs.NoExifJpg);

			lock (this.DataBase) {
				this.DataBase.MediaFiles.Count().Is(2);
				this.DataBase.MediaFiles.Check(tfs.Image1Jpg, tfs.NoExifJpg);
			}
		}

		[Test]
		public async Task ファイル作成検出() {
			using var mfm = Get.Instance<MediaFileManager>();
			var addedFiles = new List<IMediaFileModel>();
			using var are = new AutoResetEvent(false);

			// 初期ロードが終わるまで待つ
			await this.WaitTaskCompleted(2000);

			mfm.OnRegisteredMediaFiles.Subscribe(x => {
				addedFiles.AddRange(x);
				are.Set();
			});
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["0"], TestFileNames.Image1Jpg);
			are.WaitOne();
			addedFiles.Count.Is(1);
			var tfs = new TestFiles(this.TestDirectories["0"]);
			await this.WaitTaskCompleted(2000);
			addedFiles.Check(tfs.Image1Jpg);

			lock (this.DataBase) {
				this.DataBase.MediaFiles.Count().Is(1);
				this.DataBase.MediaFiles.Check(tfs.Image1Jpg);
			}
		}

		[Test]
		public void 存在しないフォルダの監視() {
			this.Settings.ScanSettings.ScanDirectories.Add(new ScanDirectory("notExistsDir"));
			var before = this.Logging.LogList.ToArray();
			using var mfm = Get.Instance<MediaFileManager>();
			mfm.LoadStates.Count().Is(2);
			var log = this.Logging.LogList.Except(before).ToArray();
			log.Any(x =>
				x.Message as string == $"監視フォルダが見つかりません。notExistsDir" &&
				x.LogLevel == LogLevel.Warning
			).IsTrue();
		}

		[Test]
		public void 対象外ファイル() {
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["0"], TestFileNames.NotTargetFile, TestFileNames.NotTargetFileNtf, TestFileNames.Image1Jpg);
			using var mfm = Get.Instance<MediaFileManager>();
			var addedFiles = new List<IMediaFileModel>();
			using var are = new AutoResetEvent(false);
			mfm.OnRegisteredMediaFiles.Subscribe(x => {
				addedFiles.AddRange(x);
				are.Set();
			});
			are.WaitOne();
			addedFiles.Count.Is(1);
			var tfs = new TestFiles(this.TestDirectories["0"]);
			addedFiles.Check(tfs.Image1Jpg);

			lock (this.DataBase) {
				this.DataBase.MediaFiles.Count().Is(1);
				this.DataBase.MediaFiles.Check(tfs.Image1Jpg);
			}
		}

		[Test]
		public void ファイルリネーム検出() {
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["0"], TestFileNames.Image1Jpg);
			using var mfm = Get.Instance<MediaFileManager>();
			var addedFiles = new List<IMediaFileModel>();
			using var are = new AutoResetEvent(false);
			mfm.OnRegisteredMediaFiles.Subscribe(x => {
				addedFiles.AddRange(x);
				are.Set();
			});
			are.WaitOne();
			addedFiles.Count.Is(1);

			var tfs = new TestFiles(this.TestDirectories["0"]);
			addedFiles.Check(tfs.Image1Jpg);

			// リネーム
			File.Move(Path.Combine(this.TestDirectories["0"], TestFileNames.Image1Jpg), Path.Combine(this.TestDirectories["0"], TestFileNames.Image2Jpg));
			are.WaitOne();
			addedFiles.Count.Is(2);
			var notExistsFile = tfs.Image1Jpg;
			notExistsFile.Exists = false;
			addedFiles.Check(new[] { notExistsFile, tfs.Image1Jpg }, false);

			lock (this.DataBase) {
				this.DataBase.MediaFiles.Count().Is(2);
				this.DataBase.MediaFiles.Check(new[] { notExistsFile, tfs.Image1Jpg }, false);
			}
		}

		[Test]
		public async Task ファイル削除検出() {
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["0"], TestFileNames.Image1Jpg);
			using var mfm = Get.Instance<MediaFileManager>();
			var addedFiles = new List<IMediaFileModel>();
			using var are = new AutoResetEvent(false);
			mfm.OnRegisteredMediaFiles.Subscribe(x => {
				addedFiles.AddRange(x);
				are.Set();
			});
			are.WaitOne();
			addedFiles.Count.Is(1);

			var tfs = new TestFiles(this.TestDirectories["0"]);
			addedFiles.Check(tfs.Image1Jpg);

			// 削除
			File.Delete(Path.Combine(this.TestDirectories["0"], TestFileNames.Image1Jpg));

			await Observable
				.Interval(TimeSpan.FromMilliseconds(100))
				.Where(_ => addedFiles.First().Exists == false)
				.Timeout(TimeSpan.FromSeconds(1))
				.FirstAsync();

			addedFiles.Count.Is(1);
			var notExistsFile = tfs.Image1Jpg;
			notExistsFile.Exists = false;
			addedFiles.Check(new[] { notExistsFile }, false);

			lock (this.DataBase) {
				this.DataBase.MediaFiles.Count().Is(1);
				this.DataBase.MediaFiles.Check(new[] { notExistsFile }, false);
			}
		}

		[Test]
		public async Task ファイル変更検出() {
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["0"], TestFileNames.Image1Jpg);
			using var mfm = Get.Instance<MediaFileManager>();
			var addedFiles = new List<IMediaFileModel>();
			using var are = new AutoResetEvent(false);
			mfm.OnRegisteredMediaFiles.Subscribe(x => {
				addedFiles.AddRange(x);
				are.Set();
			});
			are.WaitOne();
			addedFiles.Count.Is(1);

			var tfs = new TestFiles(this.TestDirectories["0"]);
			addedFiles.Check(tfs.Image1Jpg);
			lock (this.DataBase) {
				this.DataBase.MediaFiles.Count().Is(1);
				this.DataBase.MediaFiles.Check(tfs.Image1Jpg);
			}
			// 変更
			File.WriteAllBytes(
				Path.Combine(this.TestDirectories["0"], TestFileNames.Image1Jpg),
				File.ReadAllBytes(Path.Combine(this.TestDataDir, TestFileNames.NoExifJpg)));

			await Observable
				.Interval(TimeSpan.FromMilliseconds(100))
				.Where(_ => addedFiles.First().FileSize == tfs.NoExifJpg.FileSize)
				.Timeout(TimeSpan.FromSeconds(3))
				.FirstAsync();

			addedFiles.Count.Is(1);
			var after = tfs.NoExifJpg;
			after.Exists = true;
			addedFiles.Check(new[] { after }, false, false);

			lock (this.DataBase) {
				this.DataBase.MediaFiles.Count().Is(1);
				this.DataBase.MediaFiles.Check(new[] { after }, false);
			}
		}
	}
}
