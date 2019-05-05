using System.Collections.Generic;

using Livet.Messaging;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.History.Creator;
using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.ViewModels.About;
using SandBeige.MediaBox.ViewModels.Settings;
using SandBeige.MediaBox.Views;
using SandBeige.MediaBox.Views.Settings;

namespace SandBeige.MediaBox.Tests.ViewModels {
	[TestFixture]
	internal class NavigationMenuViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var vm = new NavigationMenuViewModel(new AlbumSelector());
			vm.SettingsWindowOpenCommand.IsInstanceOf<ReactiveCommand>();
			vm.AboutWindowOpenCommand.IsInstanceOf<ReactiveCommand>();
			vm.SetCurrentAlbumCommand.IsInstanceOf<ReactiveCommand<IAlbumCreator>>();
		}

		[Test]
		public void 設定ウィンドウオープン() {
			var vm = new NavigationMenuViewModel(new AlbumSelector());
			var args = new List<(object sender, InteractionMessageRaisedEventArgs e)>();
			vm.Messenger.Raised += (sender, e) => {
				args.Add((sender, e));
			};
			args.Count.Is(0);
			vm.SettingsWindowOpenCommand.Execute();
			args.Count.Is(1);
			args[0].sender.Is(vm.Messenger);
			var tm = args[0].e.Message.IsInstanceOf<TransitionMessage>();
			tm.Mode.Is(TransitionMode.NewOrActive);
			tm.WindowType.Is(typeof(SettingsWindow));
			tm.TransitionViewModel.IsInstanceOf<SettingsWindowViewModel>();
		}

		[Test]
		public void 概要ウィンドウオープン() {
			var vm = new NavigationMenuViewModel(new AlbumSelector());
			var args = new List<(object sender, InteractionMessageRaisedEventArgs e)>();
			vm.Messenger.Raised += (sender, e) => {
				args.Add((sender, e));
			};
			args.Count.Is(0);
			vm.AboutWindowOpenCommand.Execute();
			args.Count.Is(1);
			args[0].sender.Is(vm.Messenger);
			var tm = args[0].e.Message.IsInstanceOf<TransitionMessage>();
			tm.Mode.Is(TransitionMode.NewOrActive);
			tm.WindowType.Is(typeof(AboutWindow));
			tm.TransitionViewModel.IsInstanceOf<AboutWindowViewModel>();
		}

		[Test]
		public void カレントアルバム変更() {
			var selector = new AlbumSelector();
			var vm = new NavigationMenuViewModel(selector);
			selector.CurrentAlbum.Value.IsNull();
			vm.SetCurrentAlbumCommand.Execute(new FolderAlbumCreator(this.TestDataDir, this.TestDataDir));
			var fa = selector.CurrentAlbum.Value.IsInstanceOf<FolderAlbum>();
			fa.DirectoryPath.Is(this.TestDataDir);
		}
	}
}
