using System.Collections.Generic;
using System.Linq;

using Livet.Messaging;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.ViewModels.Album.Filter;

namespace SandBeige.MediaBox.Tests.ViewModels.Album.Filter {
	internal class FilterDescriptionManagerViewModelTest : ViewModelTestClassBase {
		[Test]
		public void フィルター追加削除() {
			using var vm = new FilterDescriptionManagerViewModel(new FilterDescriptionManager("main"));
			vm.FilteringConditions.Count.Is(0);
			vm.AddFilteringConditionCommand.Execute();
			vm.FilteringConditions.Count.Is(1);
			vm.AddFilteringConditionCommand.Execute();
			vm.FilteringConditions.Count.Is(2);
			vm.RemoveFilteringConditionCommand.Execute(vm.FilteringConditions.First());
			vm.FilteringConditions.Count.Is(1);
		}

		[Test]
		public void フィルター設定ウィンドウオープン() {
			using var vm = new FilterDescriptionManagerViewModel(new FilterDescriptionManager("main"));
			var args = new List<(object sender, InteractionMessageRaisedEventArgs e)>();
			vm.Messenger.Raised += (sender, e) => {
				args.Add((sender, e));
			};
			args.Count.Is(0);
			vm.OpenSetFilterWindowCommand.Execute();
			args.Count.Is(1);
			args[0].sender.Is(vm.Messenger);
			var tm = args[0].e.Message.IsInstanceOf<TransitionMessage>();
			tm.Mode.Is(TransitionMode.Normal);
			tm.WindowType.Is(typeof(MediaBox.Views.Album.Filter.SetFilterWindow));
			tm.TransitionViewModel.IsNot(vm);
		}
	}
}
