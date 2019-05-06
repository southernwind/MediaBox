using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.TestData;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileInformationsTest : ModelTestClassBase {

		[Test]
		public async Task Tags() {
			using var mfi = Get.Instance<MediaFileInformation>();

			var item1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var item2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var item3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);

			item1.AddTag("aaa");
			item1.AddTag("bbb");
			item2.AddTag("bbb");
			item3.AddTag("aaa");
			item3.AddTag("ccc");
			item3.AddTag("bbb");
			mfi.Files.Value = new[] { item1, item2, item3 };

			await mfi.WaitUpdate();
			mfi.Tags.Value.Count().Is(3);
			mfi.Tags.Value.Is(
				new ValueCountPair<string>("aaa", 2),
				new ValueCountPair<string>("bbb", 3),
				new ValueCountPair<string>("ccc", 1)
			);
		}

		[Test]
		public async Task AddTagRemoveTag() {
			using (var mfi = Get.Instance<MediaFileInformation>()) {
				await mfi.WaitUpdate();

				using var item1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
				using var item2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
				using var item3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);

				using (var tran = this.DataBase.Database.BeginTransaction()) {
					var r1 = item1.CreateDataBaseRecord();
					var r2 = item2.CreateDataBaseRecord();
					var r3 = item3.CreateDataBaseRecord();
					this.DataBase.MediaFiles.AddRange(r1, r2, r3);
					this.DataBase.SaveChanges();
					item1.MediaFileId = r1.MediaFileId;
					item2.MediaFileId = r2.MediaFileId;
					item3.MediaFileId = r3.MediaFileId;
					tran.Commit();
				}
				mfi.Files.Value = new[] { item1, item2 };
				await mfi.WaitUpdate();

				await Observable
					.Interval(TimeSpan.FromMilliseconds(0.1))
					.Where(_ => !mfi.Updating.Value)
					.Timeout(TimeSpan.FromSeconds(3))
					.FirstAsync();

				this.DataBase.Tags.Count().Is(0);

				mfi.AddTag("tag");
				mfi.AddTag("tag2");
				mfi.Files.Value = new[] { item1, item2, item3 };
				await mfi.WaitUpdate();
				mfi.AddTag("tag3");

				this.DataBase.Tags.Count().Is(3);
				var tfs = new TestFiles(this.TestDataDir);
				var test1 = tfs.Image1Jpg;
				test1.Tags = new[] { "tag", "tag2", "tag3" };
				var test2 = tfs.Image2Jpg;
				test2.Tags = new[] { "tag", "tag2", "tag3" };
				var test3 = tfs.Image3Jpg;
				test3.Tags = new[] { "tag3" };
				this.DataBase
					.MediaFiles
					.Check(test1, test2, test3);

				item1.Tags.Is("tag", "tag2", "tag3");
				item2.Tags.Is("tag", "tag2", "tag3");
				item3.Tags.Is("tag3");

				mfi.RemoveTag("tag2");

				test1.Tags = new[] { "tag", "tag3" };
				test2.Tags = new[] { "tag", "tag3" };
				test3.Tags = new[] { "tag3" };
				this.DataBase
					.MediaFiles
					.Check(test1, test2, test3);

				item1.Tags.Is("tag", "tag3");
				item2.Tags.Is("tag", "tag3");
				item3.Tags.Is("tag3");
			}
		}
	}

	internal static class _ {
		public static async Task WaitUpdate(this MediaFileInformation mediaFileInformation) {
			await Observable
				.Interval(TimeSpan.FromMilliseconds(0.1))
				.Where(_ => !mediaFileInformation.Updating.Value)
				.Timeout(TimeSpan.FromSeconds(3))
				.FirstAsync();
		}
	}
}
