using System;
using System.ComponentModel;

using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// テスト用設定インターフェイス
	/// </summary>
	public interface IForTestSettings : INotifyPropertyChanged, IDisposable {
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
		/// 設定ロード
		/// </summary>
		/// <param name="forTestSettings">読み込み元設定</param>
		void Load(IForTestSettings forTestSettings);

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		void LoadDefault();
	}
}
