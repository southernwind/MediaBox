using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileInformationsTest : TestClassBase {
		[SetUp]
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();
		}

		[Test]
		public async Task Tags() {
			using var mfi = Get.Instance<MediaFileInformation>();

			var item1 = this.MediaFactory.Create(TestFiles.Image1Jpg.FilePath);
			var item2 = this.MediaFactory.Create(TestFiles.Image2Jpg.FilePath);
			var item3 = this.MediaFactory.Create(TestFiles.Image3Jpg.FilePath);

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
		public void AddTagRemoveTag() {
			var db = Get.Instance<MediaBoxDbContext>();
			using (var mc = Get.Instance<MediaFileInformation>()) {
				var item1 = this.MediaFactory.Create(TestFiles.Image1Jpg.FilePath);
				var item2 = this.MediaFactory.Create(TestFiles.Image2Jpg.FilePath);
				var item3 = this.MediaFactory.Create(TestFiles.Image3Jpg.FilePath);

				item1.CreateDataBaseRecord();
				item2.CreateDataBaseRecord();
				item3.CreateDataBaseRecord();

				mc.Files.Value = new[] { item1, item2, item3 };

				db.Tags.Count().Is(0);

				mc.AddTag("tag");
				mc.AddTag("tag2");
				mc.AddTag("tag3");

				db.Tags.Count().Is(3);
				db.MediaFileTags.Count().Is(9);
				db.MediaFiles
					.Where(x => x.MediaFileId == 1)
					.Include(x => x.MediaFileTags)
					.ThenInclude(x => x.Tag)
					.SelectMany(x => x.MediaFileTags.Select(mft => mft.Tag.TagName))
					.Is("tag", "tag2", "tag3");
				db.MediaFileTags
					.Include(x => x.Tag)
					.Select(x => x.Tag.TagName)
					.OrderBy(x => x)
					.Is("tag", "tag", "tag", "tag2", "tag2", "tag2", "tag3", "tag3", "tag3");

				item1.Tags.Is("tag", "tag2", "tag3");
				item2.Tags.Is("tag", "tag2", "tag3");
				item3.Tags.Is("tag", "tag2", "tag3");

				mc.RemoveTag("tag2");

				item1.Tags.Is("tag", "tag3");
				item2.Tags.Is("tag", "tag3");
				item3.Tags.Is("tag", "tag3");

				db.Tags.Count().Is(3);
				db.MediaFileTags.Count().Is(6);
				db.MediaFiles
					.Where(x => x.MediaFileId == 1)
					.Include(x => x.MediaFileTags)
					.ThenInclude(x => x.Tag)
					.SelectMany(x => x.MediaFileTags.Select(mft => mft.Tag.TagName)).Is("tag", "tag3");
				db.MediaFileTags
					.Include(x => x.Tag)
					.Select(x => x.Tag.TagName)
					.OrderBy(x => x)
					.Is("tag", "tag", "tag", "tag3", "tag3", "tag3");
			}

			using (var mc = Get.Instance<MediaFileInformation>()) {
				var item4 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image4.jpg"));
				item4.CreateDataBaseRecord();
				mc.Files.Value = new[] { item4 };
				mc.AddTag("tag");

				db.Tags.Count().Is(3);

				db.MediaFiles
					.Where(x => x.MediaFileId == 4)
					.Include(x => x.MediaFileTags)
					.ThenInclude(x => x.Tag)
					.SelectMany(x => x.MediaFileTags.Select(mft => mft.Tag.TagName)).Is("tag");

				db.MediaFileTags
					.Include(x => x.Tag)
					.Select(x => x.Tag.TagName)
					.OrderBy(x => x)
					.Is("tag", "tag", "tag", "tag", "tag3", "tag3", "tag3");
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
