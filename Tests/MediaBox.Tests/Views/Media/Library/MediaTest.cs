﻿using System.Threading;
using NUnit.Framework;

namespace SandBeige.MediaBox.Tests.Views.Media.Library {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class MediaTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaBox.Views.Media.Library.Media();
		}
	}
}
