using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.Models.Settings {
	/// <summary>
	/// テスト用設定
	/// </summary>
	public class ForTestSettings : NotificationObject, IForTestSettings {
		/// <summary>
		/// 一部の処理をバックグラウンドで行うかどうか
		/// </summary>
		/// <remarks>
		/// フォアグラウンドで動かさないと通らないテストがあるので、テスト時はfalseにする。
		/// </remarks>
		public IReactiveProperty<bool> RunOnBackground {
			get;
			set;
		} = new ReactiveProperty<bool>();

		/// <summary>
		/// 設定ロード
		/// </summary>
		/// <param name="forTestSettings">読み込み元設定</param>
		public void Load(IForTestSettings forTestSettings) {
			this.RunOnBackground.Value = forTestSettings.RunOnBackground.Value;
		}

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		public void LoadDefault() {
			this.RunOnBackground.Value = true;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
			this.RunOnBackground?.Dispose();
		}
	}
}
