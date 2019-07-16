
using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.ViewModels.Tools;

namespace SandBeige.MediaBox.Tests.ViewModels.Tools {
	internal class ExternalToolViewModelTest : ViewModelTestClassBase {
		[Test]
		public void 表示名() {
			var etp = new ExternalToolParams();
			etp.DisplayName.Value = "name";
			using var model = new ExternalTool(etp);
			using var vm = new ExternalToolViewModel(model);
			vm.DisplayName.Value.Is("name");

			etp.DisplayName.Value = "dn";
			vm.DisplayName.Value.Is("dn");
		}

		[Test]
		public void コマンド() {
			var etp = new ExternalToolParams();
			etp.Command.Value = "command";
			using var model = new ExternalTool(etp);
			using var vm = new ExternalToolViewModel(model);
			vm.Command.Value.Is("command");

			etp.Command.Value = "cmd";
			vm.Command.Value.Is("cmd");
		}

		[Test]
		public void 引数() {
			var etp = new ExternalToolParams();
			etp.Arguments.Value = "args";
			using var model = new ExternalTool(etp);
			using var vm = new ExternalToolViewModel(model);
			vm.Arguments.Value.Is("args");

			etp.Arguments.Value = "arguments";
			vm.Arguments.Value.Is("arguments");
		}


		[Test]
		public void 対象拡張子() {
			var etp = new ExternalToolParams();
			etp.TargetExtensions.AddRange(".jpg", ".png");
			using var model = new ExternalTool(etp);
			using var vm = new ExternalToolViewModel(model);
			vm.TargetExtensions.Is(".jpg", ".png");

			etp.TargetExtensions.Add(".mov");
			vm.TargetExtensions.Is(".jpg", ".png", ".mov");
		}
	}
}
