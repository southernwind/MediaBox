using System;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// スキャン設定
	/// </summary>
	public interface IScanSettings : ISettingsBase, IDisposable {
		/// <summary>
		/// スキャン設定
		/// </summary>
		SettingsCollection<ScanDirectory> ScanDirectories {
			get;
		}

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		void LoadDefault();
	}
}