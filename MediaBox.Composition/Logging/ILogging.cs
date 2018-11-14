using System;

namespace SandBeige.MediaBox.Composition.Logging {
	public enum LogLevel {
		Notice,
		Warning,
		Fatal
	}

	/// <summary>
	/// エラー出力Interface
	/// </summary>
	public interface ILogging {
		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="level">ログレベル</param>
		/// <param name="message">内容</param>
		/// <param name="exception">例外オブジェクト</param>
		void Log(LogLevel Level, object message, Exception exception = null);
	}
}

