using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SandBeige.MediaBox.Views.Media.Detail;

namespace SandBeige.MediaBox.Tests.Views.Media.Detail {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class MediaListTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaList();
		}
	}
}
