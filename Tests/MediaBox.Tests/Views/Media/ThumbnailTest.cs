using System.Threading;
using NUnit.Framework;
using SandBeige.MediaBox.Views.Media;

namespace SandBeige.MediaBox.Tests.Views.Media {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class ThumbnailTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new Thumbnail();
		}
	}
}
