
using Livet.Messaging.IO;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.ViewModels.Settings.Pages;

namespace SandBeige.MediaBox.Tests.ViewModels.Settings.Pages {
	[TestFixture]
	internal class ScanSettingsViewModelTest : ViewModelTestClassBase {
		[Test]
		public void スキャン設定読み込み() {
			this.Settings.ScanSettings.ScanDirectories.Clear();
			this.Settings.ScanSettings.ScanDirectories.AddRange(
				new ScanDirectory(@"D:\test\", true, true),
				new ScanDirectory(@"F:\picture\", true, true)
			);
			using var vm = new ScanSettingsViewModel();
			vm.ScanDirectories.Is(
				new ScanDirectory(@"D:\test\", true, true),
				new ScanDirectory(@"F:\picture\", true, true)
			);
		}

		[Test]
		public void スキャン設定追加削除() {
			this.Settings.ScanSettings.ScanDirectories.Clear();
			using var vm = new ScanSettingsViewModel();
			vm.ScanDirectories.Is();
			vm.AddScanDirectoryCommand.Execute(
				new FolderSelectionMessage() {
					Response = @"C:\test\"
				});
			vm.ScanDirectories.Is(new ScanDirectory(@"C:\test\", false, true));
			this.Settings.ScanSettings.ScanDirectories.Is(new ScanDirectory(@"C:\test\", false, true));
			vm.AddScanDirectoryCommand.Execute(
				new FolderSelectionMessage() {
					Response = @"D:\test\"
				});
			vm.ScanDirectories.Is(
				new ScanDirectory(@"C:\test\", false, true),
				new ScanDirectory(@"D:\test\", false, true)
			);
			this.Settings.ScanSettings.ScanDirectories.Is(
				new ScanDirectory(@"C:\test\", false, true),
				new ScanDirectory(@"D:\test\", false, true)
			);

			vm.RemoveScanDirectoryCommand.Execute(new ScanDirectory(@"C:\test\", false, true));

			vm.ScanDirectories.Is(
				new ScanDirectory(@"D:\test\", false, true)
			);
			this.Settings.ScanSettings.ScanDirectories.Is(
				new ScanDirectory(@"D:\test\", false, true)
			);
		}
	}
}
