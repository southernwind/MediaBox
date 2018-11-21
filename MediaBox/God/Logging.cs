using log4net;
using log4net.Core;
using SandBeige.MediaBox.Composition.Logging;
using System;
using System.Threading;

namespace SandBeige.MediaBox.God {
	/// <summary>
	/// ログ出力オブジェクト
	/// </summary>
	class Logging : ILogging {
		/// <summary>
		/// Log4netインスタンス
		/// </summary>
		private readonly ILog _instance = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// エラー出力
		/// </summary>
		/// <param name="message">内容</param>
		/// <param name="exception">例外オブジェクト</param>
		public void Log(LogLevel level, object message, Exception exception = null) {
			var log4netLevel = Level.Info;
			switch (level) {
				case LogLevel.Notice:
					log4netLevel = Level.Info;
					break;
				case LogLevel.Warning:
					log4netLevel = Level.Warn;
					break;
				case LogLevel.Fatal:
					log4netLevel = Level.Fatal;
					break;
			}
			Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]{message}");
			if (exception != null) {
				Console.WriteLine(exception.StackTrace);
				Console.WriteLine(exception.Message);
			}
			this._instance.Logger.Log(this.GetType(), log4netLevel, message, exception);
		}
	}
}
