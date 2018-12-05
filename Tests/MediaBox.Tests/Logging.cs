using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SandBeige.MediaBox.Composition.Logging;

namespace SandBeige.MediaBox.Tests {
	internal class Logging : ILogging {
		public List<LogObject> LogList = new List<LogObject>();

		/// <inheritdoc />
		/// <summary>
		/// エラー出力
		/// </summary>
		/// <param name="message">内容</param>
		/// <param name="level">ログレベル</param>
		/// <param name="exception">例外オブジェクト</param>
		public void Log(object message, LogLevel level, Exception exception = null) {
			var time = DateTime.Now;
			Console.WriteLine($"[{time}][{Thread.CurrentThread.ManagedThreadId,2}]{message}");
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
