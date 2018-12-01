using Livet;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Utilities;
using Unity.Attributes;

namespace SandBeige.MediaBox.Base {
	internal class ModelBase : NotificationObject {
		protected ModelBase(){
			this.Logging = Get.Instance<ILogging>();
			this.Settings = Get.Instance<ISettings>();
			this.DataBase = Get.Instance<MediaBoxDbContext>();
		}

		/// <summary>
		/// ロガー
		/// </summary>
		protected ILogging Logging { get; set; }

		/// <summary>
		/// 設定
		/// </summary>
		protected ISettings Settings { get; set; }

		/// <summary>
		/// データベース
		/// </summary>
		protected MediaBoxDbContext DataBase { get; set; }
	}
}
