using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SandBeige.MediaBox.Views;

namespace SandBeige.MediaBox.Tests.Views {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class MainWindowTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MainWindow();
		}
	}
}
