using NUnit.Framework;

using SandBeige.MediaBox.ViewModels.Settings.Pages;

namespace SandBeige.MediaBox.Tests.ViewModels.Settings.Pages {
	[TestFixture]
	internal class GeneralSettingsViewModelTest : ViewModelTestClassBase {
		[Test]
		public void 画像拡張子() {
			this.Settings.GeneralSettings.ImageExtensions.Clear();
			this.Settings.GeneralSettings.ImageExtensions.Add(".aaa");
			using var vm = new GeneralSettingsViewModel();
			vm.ImageExtensions.Is(".aaa");
			this.Settings.GeneralSettings.ImageExtensions.Is(".aaa");
			vm.InputImageExtension.Value = ".bbb";
			vm.AddImageExtensionCommand.Execute();
			vm.ImageExtensions.Is(".aaa", ".bbb");
			this.Settings.GeneralSettings.ImageExtensions.Is(".aaa", ".bbb");
			vm.RemoveImageExtensionCommand.Execute(".aaa");
			vm.ImageExtensions.Is(".bbb");
			this.Settings.GeneralSettings.ImageExtensions.Is(".bbb");
		}

		[Test]
		public void 動画拡張子() {
			this.Settings.GeneralSettings.VideoExtensions.Clear();
			this.Settings.GeneralSettings.VideoExtensions.Add(".aaa");
			using var vm = new GeneralSettingsViewModel();
			vm.VideoExtensions.Is(".aaa");
			this.Settings.GeneralSettings.VideoExtensions.Is(".aaa");
			vm.InputVideoExtension.Value = ".bbb";
			vm.AddVideoExtensionCommand.Execute();
			vm.VideoExtensions.Is(".aaa", ".bbb");
			this.Settings.GeneralSettings.VideoExtensions.Is(".aaa", ".bbb");
		}

		[Test]
		public void マップapiキー() {
			this.Settings.GeneralSettings.BingMapApiKey.Value = "ABCDEF";
			using var vm = new GeneralSettingsViewModel();
			vm.BingMapApiKey.Value.Is("ABCDEF");
			this.Settings.GeneralSettings.BingMapApiKey.Value.Is("ABCDEF");

			vm.BingMapApiKey.Value = "Key";
			vm.BingMapApiKey.Value.Is("Key");
			this.Settings.GeneralSettings.BingMapApiKey.Value.Is("Key");
		}

		[Test]
		public void サムネイル横幅() {
			this.Settings.GeneralSettings.ThumbnailHeight.Value = 55;
			using var vm = new GeneralSettingsViewModel();
			vm.ThumbnailHeight.Value.Is(55);
			this.Settings.GeneralSettings.ThumbnailHeight.Value.Is(55);

			vm.ThumbnailHeight.Value = 38;
			vm.ThumbnailHeight.Value.Is(38);
			this.Settings.GeneralSettings.ThumbnailHeight.Value.Is(38);
		}

		[Test]
		public void サムネイル高さ() {
			this.Settings.GeneralSettings.ThumbnailWidth.Value = 55;
			using var vm = new GeneralSettingsViewModel();
			vm.ThumbnailWidth.Value.Is(55);
			this.Settings.GeneralSettings.ThumbnailWidth.Value.Is(55);

			vm.ThumbnailWidth.Value = 38;
			vm.ThumbnailWidth.Value.Is(38);
			this.Settings.GeneralSettings.ThumbnailWidth.Value.Is(38);
		}

		[Test]
		public void マップピンサイズ() {
			this.Settings.GeneralSettings.MapPinSize.Value = 55;
			using var vm = new GeneralSettingsViewModel();
			vm.MapPinSize.Value.Is(55);
			this.Settings.GeneralSettings.MapPinSize.Value.Is(55);

			vm.MapPinSize.Value = 38;
			vm.MapPinSize.Value.Is(38);
			this.Settings.GeneralSettings.MapPinSize.Value.Is(38);
		}
	}
}
