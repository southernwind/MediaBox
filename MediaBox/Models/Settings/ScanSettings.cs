
using System;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Models.Settings {
	public class ScanSettings : SettingsBase, IScanSettings {
		/// <summary>
		/// スキャン設定
		/// </summary>
		public SettingsCollection<ScanDirectory> ScanDirectories {
			get;
		} = new SettingsCollection<ScanDirectory>(
			new ScanDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures, Environment.SpecialFolderOption.DoNotVerify), true, true)
		);
	}
}
