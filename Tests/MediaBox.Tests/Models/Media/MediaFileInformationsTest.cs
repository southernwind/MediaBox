using System.IO;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileInformationsTest : TestClassBase {
		[Test]
		public void Tags() {
			using (var mc = Get.Instance<MediaFileInformations>()) {
				var item1 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
				item1.AddTag("aaa");
				item1.AddTag("bbb");
				var item2 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image2.jpg"));
				item2.AddTag("bbb");
				var item3 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image3.jpg"));
				item3.AddTag("aaa");
				item3.AddTag("ccc");
				item3.AddTag("bbb");
				mc.Files.Value = new[] { item1, item2, item3 };
				mc.Tags.Value.Count().Is(3);
				mc.Tags.Value.Single(x => x.Value == "aaa").Count.Is(2);
				mc.Tags.Value.Single(x => x.Value == "bbb").Count.Is(3);
				mc.Tags.Value.Single(x => x.Value == "ccc").Count.Is(1);
			}
		}

		[Test]
		public void AddTagRemoveTag() {
			var db = Get.Instance<MediaBoxDbContext>();
			using (var mc = Get.Instance<MediaFileInformations>()) {
				var item1 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
				var item2 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image2.jpg"));
				var item3 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image3.jpg"));
				var item4 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image4.jpg"));
				item1.GetFileInfo();
				item2.GetFileInfo();
				item3.GetFileInfo();
				item1.RegisterToDataBase();
				item2.RegisterToDataBase();
				item3.RegisterToDataBase();

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

			using (var mc = Get.Instance<MediaFileInformations>()) {
				var item4 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image4.jpg"));
				item4.GetFileInfo();
				item4.RegisterToDataBase();
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
		}
	}
}
