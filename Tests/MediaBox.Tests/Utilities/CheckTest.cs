using NUnit.Framework;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;
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
		public void 対象拡張子テスト(bool result, string path) {
			TypeRegistrations.RegisterType(new UnityContainer());
			var settings = Get.Instance<ISettings>();
			settings.GeneralSettings.ImageExtensions.Clear();
			settings.GeneralSettings.ImageExtensions.AddRange(new[] { ".jpg" });
			settings.GeneralSettings.VideoExtensions.Clear();
			settings.GeneralSettings.VideoExtensions.AddRange(new[] { ".mp4", ".abc" });
			path.IsTargetExtension().Is(result);
		}

		[TestCase(false, "")]
		[TestCase(false, @"c:\image.jpeg")]
		[TestCase(false, @"c:\image.jpg")]
		[TestCase(false, @"c:\image.mp3")]
		[TestCase(true, @"c:\image.mp4")]
		[TestCase(true, @"c:\image.abc")]
		[TestCase(true, @"file.mp4")]
		public void 動画拡張子テスト(bool result, string path) {
			TypeRegistrations.RegisterType(new UnityContainer());
			var settings = Get.Instance<ISettings>();
			settings.GeneralSettings.ImageExtensions.Clear();
			settings.GeneralSettings.ImageExtensions.AddRange(new[] { ".jpg" });
			settings.GeneralSettings.VideoExtensions.Clear();
			settings.GeneralSettings.VideoExtensions.AddRange(new[] { ".mp4", ".abc" });
			path.IsVideoExtension().Is(result);
		}
	}
}
