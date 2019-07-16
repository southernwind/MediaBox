using System;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Utilities;

using Unity;

namespace SandBeige.MediaBox.Tests.God {
	[TestFixture]
	internal class LoggingTest {
		[SetUp]
		public void SetUp() {
			TypeRegistrations.RegisterType(new UnityContainer());
		}

		[Test]
		public void Log() {
			// とりあえず色んなパターン通して例外出なければOK
			var log = Get.Instance<ILogging>();
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
	}
}
