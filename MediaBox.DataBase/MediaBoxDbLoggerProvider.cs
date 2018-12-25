using System;

using Microsoft.Extensions.Logging;

namespace SandBeige.MediaBox.DataBase {
	/// <summary>
	/// ログ出力クラス
	/// </summary>
	internal class MediaBoxDbLoggerProvider : ILoggerProvider {
		public ILogger CreateLogger(string categoryName) {
			return new ConsoleLogger();
		}

		public void Dispose() {
		}

		private class ConsoleLogger : ILogger {
			public IDisposable BeginScope<TState>(TState state) {
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
