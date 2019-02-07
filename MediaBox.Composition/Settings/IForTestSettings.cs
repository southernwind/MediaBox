using System;

using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// テスト用設定インターフェイス
	/// </summary>
	public interface IForTestSettings : ISettingsBase, IDisposable {
		/// <summary>
		/// 一部の処理をバックグラウンドで行うかどうか
		/// </summary>
		/// <remarks>
		/// フォアグラウンドで動かさないと通らないテストがあるので、テスト時はfalseにする。
		/// </remarks>
		SettingsItem<bool> RunOnBackground {
			get;
		}

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		void LoadDefault();
	}
}
