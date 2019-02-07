using System.Windows;

using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Models.States {
	/// <summary>
	/// サイズ状態
	/// </summary>
	public class SizeStates : SettingsBase {
		/// <summary>
		/// メインウィンドウ幅
		/// </summary>
		public SettingsItem<double> MainWindowWidth {
			get;
			set;
		} = new SettingsItem<double>(1920);

		/// <summary>
		/// メインウィンドウ高さ
		/// </summary>
		public SettingsItem<double> MainWindowHeight {
			get;
			set;
		} = new SettingsItem<double>(1080);

		/// <summary>
		/// メインウィンドウ左上X
		/// </summary>
		public SettingsItem<double> MainWindowLeft {
			get;
			set;
		} = new SettingsItem<double>(double.NaN);

		/// <summary>
		/// メインウィンドウ左上Y
		/// </summary>
		public SettingsItem<double> MainWindowTop {
			get;
			set;
		} = new SettingsItem<double>(50);

		/// <summary>
		/// メインウィンドウ状態
		/// </summary>
		public SettingsItem<WindowState> MainWindowState {
			get;
			set;
		} = new SettingsItem<WindowState>(WindowState.Normal);

		/// <summary>
		/// デフォルトロード
		/// </summary>
		public void LoadDefault() {
			this.MainWindowWidth.SetDefaultValue();
			this.MainWindowHeight.SetDefaultValue();
			this.MainWindowLeft.SetDefaultValue();
			this.MainWindowTop.SetDefaultValue();
			this.MainWindowState.SetDefaultValue();
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
			this.MainWindowWidth?.Dispose();
			this.MainWindowHeight?.Dispose();
			this.MainWindowLeft?.Dispose();
			this.MainWindowTop?.Dispose();
			this.MainWindowState?.Dispose();
		}
	}
}
