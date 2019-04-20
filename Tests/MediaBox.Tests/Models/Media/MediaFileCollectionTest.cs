
using Livet;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileCollectionTest : TestClassBase {
		[SetUp]
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();
		}

		[Test]
		public void アイテム追加削除() {
			var image1 = this.MediaFactory.Create(TestFiles.Image1Jpg.FilePath);
			var image2 = this.MediaFactory.Create(TestFiles.Image2Jpg.FilePath);
			var image3 = this.MediaFactory.Create(TestFiles.Image3Jpg.FilePath);

			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			osc.Add(image1);
			using var mc = Get.Instance<MediaFileCollection>(osc);
			mc.Count.Value.Is(1);
			mc.Items.Is(image1);
			osc.Is(image1);

			// 追加
			mc.Items.Add(image2);
			osc.Add(image3);
			mc.Count.Value.Is(3);
			mc.Items.Is(image1, image2, image3);
			osc.Is(image1, image2, image3);

			// 削除
			mc.Items.Remove(image2);
			mc.Count.Value.Is(2);
			mc.Items.Is(image1, image3);
			osc.Is(image1, image3);
		}
	}
}
