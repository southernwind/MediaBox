using System;

using Microsoft.Extensions.Logging;

namespace SandBeige.MediaBox.DataBase {
	/// <summary>
	/// ログ出力クラス
	/// </summary>
	internal class MediaBoxDbLoggerProvider : ILoggerProvider {
		/// <summary>
		/// ロガーの作成
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public ILogger CreateLogger(string categoryName) {
			return new ConsoleLogger();
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose() {
		}

		/// <summary>
		/// コンソール出力ロガー
		/// </summary>
		private class ConsoleLogger : ILogger {
			public IDisposable? BeginScope<TState>(TState state) {
				return null;
			}

			public bool IsEnabled(LogLevel logLevel) {
				return logLevel > LogLevel.Debug;
			}

			public void Log<TState>(
				LogLevel logLevel,
				EventId eventId,
				TState state,
				Exception exception,
				Func<TState, Exception, string> formatter) {
				Console.WriteLine(formatter(state, exception));
			}
		}
	}
}
