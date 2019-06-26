using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.ViewModels.Settings.Pages;

namespace SandBeige.MediaBox.Tests.ViewModels.Settings.Pages {
	[TestFixture]
	internal class ExternalToolsSettingsViewModelTest : ViewModelTestClassBase {
		[Test]
		public void 画像拡張子() {
			this.Settings.GeneralSettings.ImageExtensions.Clear();
			this.Settings.GeneralSettings.ImageExtensions.AddRange(".aaa", ".bbb", ".ccc");
			using var vm = new ExternalToolsSettingsViewModel();
			vm.CanditateImageExtensions.Select(x => x.Extension.Value).Is(".aaa", ".bbb", ".ccc");
			this.Settings.GeneralSettings.ImageExtensions.Add(".ddd");
			this.Settings.GeneralSettings.ImageExtensions.Remove(".bbb");
			vm.CanditateImageExtensions.Select(x => x.Extension.Value).Is(".aaa", ".ccc", ".ddd");
		}

		[Test]
		public void 動画拡張子() {
			this.Settings.GeneralSettings.VideoExtensions.Clear();
			this.Settings.GeneralSettings.VideoExtensions.AddRange(".aaa", ".bbb", ".ccc");
			using var vm = new ExternalToolsSettingsViewModel();
			vm.CanditateVideoExtensions.Select(x => x.Extension.Value).Is(".aaa", ".bbb", ".ccc");
			this.Settings.GeneralSettings.VideoExtensions.Add(".ddd");
			this.Settings.GeneralSettings.VideoExtensions.Remove(".bbb");
			vm.CanditateVideoExtensions.Select(x => x.Extension.Value).Is(".aaa", ".ccc", ".ddd");
		}
	}
}
