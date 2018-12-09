using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SandBeige.MediaBox.Views.SubWindows.OptionWindow.Pages;

namespace SandBeige.MediaBox.Tests.Views.SubWindows.OptionWindow.Pages {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class GeneralSettingsTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new GeneralSettings();
		}
	}
}
