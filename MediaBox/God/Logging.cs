using System;
using System.Threading;

using log4net;
using log4net.Core;

using SandBeige.MediaBox.Composition.Logging;

namespace SandBeige.MediaBox.God {
	using System.IO;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// ログ出力オブジェクト
	/// </summary>
	public class Logging : ILogging {
		/// <summary>
		/// Log4netインスタンス
		/// </summary>
		private readonly ILog _instance = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// エラー出力
		/// </summary>
		/// <param name="message">内容</param>
		/// <param name="level">ログ出力レベル</param>
		/// <param name="exception">例外オブジェクト</param>
		/// <param name="file">ファイル名</param>
		/// <param name="line">行数</param>
		/// <param name="member">メンバー名</param>
		public void Log(
			object message,
			LogLevel level = LogLevel.Info,
			Exception exception = null,
			[CallerFilePath] string file = null,
			[CallerLineNumber] int line = 0,
			[CallerMemberName] string member = null) {
			Level log4NetLevel;
			switch (level) {
				case LogLevel.Trace:
					log4NetLevel = Level.Trace;
					break;
				case LogLevel.Debug:
					log4NetLevel = Level.Debug;
					break;
				case LogLevel.Info:
					log4NetLevel = Level.Info;
					break;
				case LogLevel.Warning:
					log4NetLevel = Level.Warn;
					break;
				case LogLevel.Error:
					log4NetLevel = Level.Error;
					break;
				case LogLevel.Fatal:
					log4NetLevel = Level.Fatal;
					break;
				default:
					throw new ArgumentException();
			}
			var time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
			Console.WriteLine($"[{time}][{Thread.CurrentThread.ManagedThreadId,2}][{Path.GetFileName(file)}:{line}({member})]{message}");
			if (exception != null) {
				Console.WriteLine($"[{time}]{exception}");
			}
			this._instance.Logger.Log(this.GetType(), log4NetLevel, $"[{Path.GetFileName(file)}:{line}({member})]" + message, exception);
		}
	}
}
