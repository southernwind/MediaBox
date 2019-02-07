
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Models.Settings {
	/// <summary>
	/// テスト用設定
	/// </summary>
	public class ForTestSettings : SettingsBase, IForTestSettings {
		/// <summary>
		/// 一部の処理をバックグラウンドで行うかどうか
		/// </summary>
		/// <remarks>
		/// フォアグラウンドで動かさないと通らないテストがあるので、テスト時はfalseにする。
		/// </remarks>
		public SettingsItem<bool> RunOnBackground {
			get;
			set;
		} = new SettingsItem<bool>(true);

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		public void LoadDefault() {
			this.RunOnBackground.SetDefaultValue();
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
			this.RunOnBackground?.Dispose();
		}
	}
}
