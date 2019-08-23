namespace SandBeige.MediaBox.Models.Notification {
	internal interface INotification {
		/// <summary>
		/// イメージソース
		/// </summary>
		object ImageSource {
			get;
		}

		/// <summary>
		/// メッセージ
		/// </summary>
		string Message {
			get;
		}
	}
}
