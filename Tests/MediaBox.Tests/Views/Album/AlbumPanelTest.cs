using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SandBeige.MediaBox.Views.Album;

namespace SandBeige.MediaBox.Tests.Views.Album {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class AlbumPanelTest {
		[Test]
		public void Test() {
			_ = new AlbumPanel();
		}
	}
}
