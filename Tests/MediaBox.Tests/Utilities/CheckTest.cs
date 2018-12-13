using NUnit.Framework;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Utilities;
using Unity;

namespace SandBeige.MediaBox.Tests.Utilities {
	[TestFixture]
	internal class CheckTest {

		[TestCase(false, "")]
		[TestCase(false, @"c:\image.jpeg")]
		[TestCase(true, @"c:\image.jpg")]
		[TestCase(false, @"c:\image.mp3")]
		[TestCase(true, @"c:\image.mp4")]
		[TestCase(true, @"c:\image.abc")]
		[TestCase(true, @"file.mp4")]
		public void IsTargetExtension(bool result, string path) {
			TypeRegistrations.RegisterType(new UnityContainer());
			var settings = Get.Instance<ISettings>();
			settings.GeneralSettings.TargetExtensions.Value = new[] { ".jpg", ".mp4", ".abc" };
			Assert.AreEqual(result, path.IsTargetExtension());
		}
	}
}
