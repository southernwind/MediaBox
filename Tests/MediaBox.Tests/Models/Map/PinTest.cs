using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Tests.Views;

namespace SandBeige.MediaBox.Tests.Models.Map {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class PinTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new Pin();
		}
	}
}
