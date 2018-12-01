using System;

namespace SandBeige.MediaBox.Composition.Logging {
	/// <summary>
	/// ログレベル
	/// </summary>
	public enum LogLevel {
		/// <summary>
		/// 通知
		/// </summary>
		Notice,
		/// <summary>
		/// 警告
		/// </summary>
		Warning,
		/// <summary>
		/// 致命的
		/// </summary>
		Fatal
	}

	/// <summary>
	/// エラー出力Interface
	/// </summary>
	public interface ILogging {
		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="message">内容</param>
		/// <param name="level">ログレベル</param>
		/// <param name="exception">例外オブジェクト</param>
		void Log(object message, LogLevel Level = LogLevel.Notice, Exception exception = null);
	}
}

