using System.Linq;
using System.Windows;

using NUnit.Framework;

using SandBeige.MediaBox.ViewModels.Dialog;

namespace SandBeige.MediaBox.Tests.ViewModels.Dialog {
	internal class DialogViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var vm = new DialogViewModel("title", "message", MessageBoxButton.YesNo);
			vm.Title.Value.Is("title");
			vm.Message.Value.Is("message");
		}

		[TestCase(MessageBoxButton.OK, MessageBoxResult.OK)]
		[TestCase(MessageBoxButton.OKCancel, MessageBoxResult.OK, MessageBoxResult.Cancel)]
		[TestCase(MessageBoxButton.YesNo, MessageBoxResult.Yes, MessageBoxResult.No)]
		[TestCase(MessageBoxButton.YesNoCancel, MessageBoxResult.Yes, MessageBoxResult.No, MessageBoxResult.Cancel)]
		public void ボタンパターン(MessageBoxButton button, params MessageBoxResult[] result) {
			var vm = new DialogViewModel("title", "message", button);
			vm.ButtonList.Select(x => x.DialogResult).Is(result);
		}

		[Test]
		public void デフォルトボタンYes() {
			var vm = new DialogViewModel("title", "message", MessageBoxButton.YesNoCancel, MessageBoxResult.Yes);
			vm.ButtonList.Single(x => x.IsDefault).DialogResult.Is(MessageBoxResult.Yes);

		}

		[Test]
		public void デフォルトボタンNo() {
			var vm = new DialogViewModel("title", "message", MessageBoxButton.YesNoCancel, MessageBoxResult.No);
			vm.ButtonList.Single(x => x.IsDefault).DialogResult.Is(MessageBoxResult.No);

		}

		[Test]
		public void デフォルトボタンCancel() {
			var vm = new DialogViewModel("title", "message", MessageBoxButton.YesNoCancel, MessageBoxResult.Cancel);
			vm.ButtonList.Single(x => x.IsDefault).DialogResult.Is(MessageBoxResult.Cancel);
		}

		[Test]
		public void デフォルトボタンok() {
			var vm = new DialogViewModel("title", "message", MessageBoxButton.OKCancel, MessageBoxResult.OK);
			vm.ButtonList.Single(x => x.IsDefault).DialogResult.Is(MessageBoxResult.OK);
		}
	}
}
