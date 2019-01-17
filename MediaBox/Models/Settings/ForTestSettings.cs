using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.Models.Settings {
	public class ForTestSettings : NotificationObject, IForTestSettings {
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
		/// 設定ロード
		/// </summary>
		public void Load() {
			this.RunOnBackground.Value = true;
		}

		public void Dispose() {
			this.RunOnBackground?.Dispose();
		}
	}
}
