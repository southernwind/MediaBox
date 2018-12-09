using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SandBeige.MediaBox.Views.Media;

namespace SandBeige.MediaBox.Tests.Views.Media {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class MediaViewTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaView();
		}
	}
}
