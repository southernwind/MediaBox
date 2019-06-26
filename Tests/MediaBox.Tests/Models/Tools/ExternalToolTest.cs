
using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Tools {
	internal class ExternalToolTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.UseFileSystem();
		}

		[Test]
		public void 表示名() {
			var param = new ExternalToolParams();
			param.DisplayName.Value = "name";

			using var et = new ExternalTool(param);
			et.DisplayName.Value.Is("name");

			param.DisplayName.Value = "name2";
			et.DisplayName.Value.Is("name2");
		}

		[Test]
		public void コマンド() {
			var param = new ExternalToolParams();
			param.Command.Value = "command";

			using var et = new ExternalTool(param);
			et.Command.Value.Is("command");

			param.Command.Value = "command2";
			et.Command.Value.Is("command2");
		}

		[Test]
		public void 引数() {
			var param = new ExternalToolParams();
			param.Arguments.Value = "arg";

			using var et = new ExternalTool(param);
			et.Arguments.Value.Is("arg");

			param.Arguments.Value = "arg2";
			et.Arguments.Value.Is("arg2");
		}

		[Test]
		public void 対象拡張子() {
			var param = new ExternalToolParams();
			param.TargetExtensions.AddRange(".jpg", ".png", ".gif");

			using var et = new ExternalTool(param);
			et.TargetExtensions.Is(".jpg", ".png", ".gif");

			param.TargetExtensions.AddRange(".mov", ".mp4");
			RxUtility.WaitScheduler(ReactivePropertyScheduler.Default);
			et.TargetExtensions.Is(".jpg", ".png", ".gif", ".mov", ".mp4");
		}
	}
}
