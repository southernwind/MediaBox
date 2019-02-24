using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

using SandBeige.MediaBox.Composition.Logging;

namespace SandBeige.MediaBox.Tests {
	internal class Logging : ILogging {
		public readonly List<LogObject> LogList = new List<LogObject>();

		/// <summary>
		/// エラー出力
		/// </summary>
		/// <param name="message">内容</param>
		/// <param name="level">ログ出力レベル</param>
		/// <param name="exception">例外オブジェクト</param>
		public void Log(
			object message,
			LogLevel level,
			Exception exception = null,
			[CallerFilePath] string file = null,
			[CallerLineNumber] int line = 0,
			[CallerMemberName] string member = null) {
			var time = DateTime.Now;
			Console.WriteLine($"[{time}][{Thread.CurrentThread.ManagedThreadId,2}][{file}:{line}({member})]{message}");
			if (exception != null) {
				Console.WriteLine($"[{time}]{exception.StackTrace}");
				Console.WriteLine($"[{time}]{exception.Message}");
			}

			this.LogList.Add(new LogObject(time, message, level, exception));
		}

		public class LogObject {
			public LogObject(DateTime dateTime, object message, LogLevel level, Exception exception) {
				this.DateTime = dateTime;
				this.Message = message;
				this.LogLevel = level;
				this.Exception = exception;
			}

			public DateTime DateTime {
				get;
			}

			public object Message {
				get;
			}

			public LogLevel LogLevel {
				get;
			}

			public Exception Exception {
				get;
			}
		}
	}
}
