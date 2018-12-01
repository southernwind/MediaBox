using Livet;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Utilities;
using Unity.Attributes;

namespace SandBeige.MediaBox.Base {
	internal class ViewModelBase : ViewModel {
		protected ViewModelBase() {
			this.Logging = Get.Instance<ILogging>();
			this.Settings = Get.Instance<ISettings>();
		}
		/// <summary>
		/// ロガー
		/// </summary>
		protected ILogging Logging { get; set; }

		/// <summary>
		/// 設定
		/// </summary>
		protected ISettings Settings { get; set; }
	}
}
