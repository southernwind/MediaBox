using System.IO;
using System.Linq;
using NUnit.Framework;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFilePropertiesTest : TestClassBase{
		[Test]
		public void Tags() {
			using (var mc = Get.Instance<MediaFileProperties>()) {
				var item1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
				item1.Tags.Add("aaa");
				item1.Tags.Add("bbb");
				var item2 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
				item2.Tags.Add("bbb");
				var item3 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
				item3.Tags.Add("aaa");
				item3.Tags.Add("ccc");
				item3.Tags.Add("bbb");
				mc.Items.Add(item1);
				mc.Items.Add(item2);
				mc.Items.Add(item3);
				mc.Tags.Value.Count().Is(3);
				mc.Tags.Value.Single(x => x.Value == "aaa").Count.Is(2);
				mc.Tags.Value.Single(x => x.Value == "bbb").Count.Is(3);
				mc.Tags.Value.Single(x => x.Value == "ccc").Count.Is(1);
			}
		}

		[Test]
		public void AddTagRemoveTag() {
			using (var mc = Get.Instance<MediaFileProperties>()) 
			using (var album = Get.Instance<RegisteredAlbum>()) {
				album.Create();
				var item1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
				var item2 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
				var item3 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));

				mc.Items.Add(item1);
				mc.Items.Add(item2);
				mc.Items.Add(item3);
				
				mc.AddTag("tag");
				mc.AddTag("tag2");
				mc.AddTag("tag3");

				// TODO : 対象のメディアがDBに登録されていないとタグ登録が動かなくて確認できないのでやり方を考える
				// item1.Tags.Is("tag", "tag2", "tag3");
				// item2.Tags.Is("tag", "tag2", "tag3");
				// item3.Tags.Is("tag", "tag2", "tag3");

				mc.RemoveTag("tag2");

				// item1.Tags.Is("tag", "tag3");
				// item2.Tags.Is("tag", "tag3");
				// item3.Tags.Is("tag", "tag3");
			}
		}
	}
}
