using System.Windows;

using Livet;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Models.States {
	/// <summary>
	/// サイズ状態
	/// </summary>
	public class SizeStates : NotificationObject {
		/// <summary>
		/// メインウィンドウ幅
		/// </summary>
		public IReactiveProperty<double> MainWindowWidth {
			get;
			set;
		} = new ReactiveProperty<double>();

		/// <summary>
		/// メインウィンドウ高さ
		/// </summary>
		public IReactiveProperty<double> MainWindowHeight {
			get;
			set;
		} = new ReactiveProperty<double>();

		/// <summary>
		/// メインウィンドウ左上X
		/// </summary>
		public IReactiveProperty<double> MainWindowLeft {
			get;
			set;
		} = new ReactiveProperty<double>();

		/// <summary>
		/// メインウィンドウ左上Y
		/// </summary>
		public IReactiveProperty<double> MainWindowTop {
			get;
			set;
		} = new ReactiveProperty<double>();

		/// <summary>
		/// メインウィンドウ状態
		/// </summary>
		public IReactiveProperty<WindowState> MainWindowState {
			get;
			set;
		} = new ReactiveProperty<WindowState>();

		/// <summary>
		/// ロード
		/// </summary>
		/// <param name="sizeStates"></param>
		public void Load(SizeStates sizeStates) {
			this.MainWindowWidth.Value = sizeStates.MainWindowWidth.Value;
			this.MainWindowHeight.Value = sizeStates.MainWindowHeight.Value;
			this.MainWindowLeft.Value = sizeStates.MainWindowLeft.Value;
			this.MainWindowTop.Value = sizeStates.MainWindowTop.Value;
			this.MainWindowState.Value = sizeStates.MainWindowState.Value == WindowState.Minimized ? WindowState.Normal : sizeStates.MainWindowState.Value;
		}

		/// <summary>
		/// デフォルトロード
		/// </summary>
		public void LoadDefault() {
			this.MainWindowWidth.Value = 1920;
			this.MainWindowHeight.Value = 1080;
			this.MainWindowLeft.Value = double.NaN;
			this.MainWindowTop.Value = double.NaN;
			this.MainWindowState.Value = WindowState.Normal;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
			this.MainWindowWidth?.Dispose();
			this.MainWindowHeight?.Dispose();
		}
	}
}
