using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SandBeige.MediaBox.Tests.Views.SubWindows.AlbumCreateWindow {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class AlbumCreateWindowTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaBox.Views.SubWindows.AlbumCreateWindow.AlbumCreateWindow();
		}
	}
}
