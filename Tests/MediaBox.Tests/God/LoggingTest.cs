using System;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Logging;

namespace SandBeige.MediaBox.Tests.God {
	[TestFixture]
	internal class LoggingTest {

		[Test]
		public void Log() {
			// とりあえず色んなパターン通して例外出なければOK
			var log = new MediaBox.God.Logging();
			log.Log("message");
			log.Log(55);
			log.Log(new object());
			log.Log("message", LogLevel.Trace);
			log.Log("message", LogLevel.Debug);
			log.Log("message", LogLevel.Info);
			log.Log("message", LogLevel.Warning);
			log.Log("message", LogLevel.Error);
			log.Log("message", LogLevel.Fatal);
			log.Log("message", LogLevel.Fatal, new Exception("message"));
		}

		[Test]
		public void 異常パラメータ1() {
			var log = new MediaBox.God.Logging();
			Assert.Throws<ArgumentException>(() => {
				log.Log("message", (LogLevel)1000);
			});
		}
	}
}
