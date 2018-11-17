using Livet;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using Unity.Attributes;

namespace SandBeige.MediaBox.Base {
	internal class ViewModelBase : ViewModel {
		/// <summary>
		/// ロガー
		/// </summary>
		[Dependency]
		protected ILogging Logging { get; set; }

		/// <summary>
		/// 設定
		/// </summary>
		[Dependency]
		protected ISettings Settings { get; set; }
	}
}
