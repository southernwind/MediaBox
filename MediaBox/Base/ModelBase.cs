using Livet;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using Unity.Attributes;

namespace SandBeige.MediaBox.Base {
	internal class ModelBase : NotificationObject {
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

		/// <summary>
		/// データベース
		/// </summary>
		[Dependency]
		protected MediaBoxDbContext DataBase { get; set; }
	}
}
