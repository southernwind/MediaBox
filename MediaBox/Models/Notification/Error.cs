namespace SandBeige.MediaBox.Models.Notification {
	public class Error : INotification {
		/// <summary>
		/// イメージソース
		/// </summary>
		public object ImageSource {
			get;
		}

		/// <summary>
		/// メッセージ
		/// </summary>
		public string Message {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="imageSource">イメージソース</param>
		/// <param name="message">メッセージ</param>
		public Error(object imageSource, string message) {
			this.ImageSource = imageSource;
			this.Message = message;
		}
	}
}
