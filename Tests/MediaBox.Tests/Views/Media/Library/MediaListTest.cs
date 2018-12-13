using System.Threading;
using NUnit.Framework;
using SandBeige.MediaBox.Views.Media.Library;

namespace SandBeige.MediaBox.Tests.Views.Media.Library {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class MediaListTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaList();
		}
	}
}
