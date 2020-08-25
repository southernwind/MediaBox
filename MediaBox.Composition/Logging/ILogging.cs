using System;

namespace SandBeige.MediaBox.Composition.Logging {
	using System.Runtime.CompilerServices;

	/// <summary>
	/// ログレベル
	/// </summary>
	public enum LogLevel {
		/// <summary>
		/// トレース
		/// </summary>
		Trace,
		/// <summary>
		/// デバッグ
		/// </summary>
		Debug,
		/// <summary>
		/// 情報
		/// </summary>
		Info,
		/// <summary>
		/// 警告
		/// </summary>
		Warning,
		/// <summary>
		/// エラー
		/// </summary>
		Error,
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
		/// <param name="file">ソースファイル名</param>
		/// <param name="line">呼び出し元行数</param>
		/// <param name="number">呼び出し元メソッド名</param>
		void Log(
			object? message,
			LogLevel level = LogLevel.Info,
			Exception? exception = null,
			[CallerFilePath] string? file = null,
			[CallerLineNumber] int line = 0,
			[CallerMemberName] string? number = null);
	}
}

