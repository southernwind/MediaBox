﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SandBeige.MediaBox.Tests.Views.Media.InformationWindow {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class InformationWindowTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaBox.Views.Media.InfromationWindow.InformationWindow();
		}
	}
}