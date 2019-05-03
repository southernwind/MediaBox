using System.Windows;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.Tests.Models.Map {
	[TestFixture]
	internal class MapPinTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();
		}

		[Test]
		public void テスト() {
			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var rect = new Rectangle(new Point(5, 15), new Size(10, 8));
			var mg = new MapPin(image1, rect);
			mg.Items.Add(image2);
			mg.Items.Add(image3);

			mg.Core.Value.Is(image1);
			mg.CoreRectangle.Is(rect);
			mg.Count.Value.Is(3);
			mg.Items.Is(image1, image2, image3);
		}
	}
}
