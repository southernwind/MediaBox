namespace SandBeige.MediaBox.Composition.Interfaces.Models.Notification {
	public interface INotification {
		/// <summary>
		/// イメージソース
		/// </summary>
		object? ImageSource {
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
