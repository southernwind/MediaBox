using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;

using Livet.Messaging;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	[TestFixture]
	internal class AlbumSelectorViewModelTest : ViewModelTestClassBase {
		public override void SetUp() {
			base.SetUp();
			ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
		}

		[Test]
		public void アルバムリスト() {
			var ra1 = new RegisteredAlbum();
			var ra2 = new RegisteredAlbum();
			var ra3 = new RegisteredAlbum();
			var ac = Get.Instance<AlbumContainer>();
			var vm = new AlbumSelectorViewModel();
			vm.AlbumList.Is();
			ac.AddAlbum(ra1);
			vm.AlbumList.Select(x => x.Model).Is(ra1);
			ac.AddAlbum(ra2);
			vm.AlbumList.Select(x => x.Model).Is(ra1, ra2);
			ac.AddAlbum(ra3);
			vm.AlbumList.Select(x => x.Model).Is(ra1, ra2, ra3);
			ac.RemoveAlbum(ra2);
			vm.AlbumList.Select(x => x.Model).Is(ra1, ra3);
		}

		[Test]
		public void カレントアルバム変更() {
			var ra1 = new RegisteredAlbum();
			ra1.AlbumId.Value = 1;
			ra1.Title.Value = "title1";
			var ra2 = new RegisteredAlbum();
			ra2.AlbumId.Value = 2;
			ra2.Title.Value = "title2";
			var ra3 = new RegisteredAlbum();
			ra3.AlbumId.Value = 3;
			ra3.Title.Value = "title3";
			var ac = Get.Instance<AlbumContainer>();
			var vm = new AlbumSelectorViewModel();
			ac.AddAlbum(ra1);
			ac.AddAlbum(ra2);
			ac.AddAlbum(ra3);
			vm.CurrentAlbum.Value.IsNull();
			vm.SetAlbumToCurrent.Execute(this.ViewModelFactory.Create(ra2));
			vm.CurrentAlbum.Value.Model.Is(ra2);
		}

		[Test]
		public void カレントフォルダアルバム変更() {
			var vm = new AlbumSelectorViewModel();
			vm.FolderAlbumPath.Value = this.TestDataDir;
			vm.SetFolderAlbumToCurrent.Execute();
			var fa = vm.CurrentAlbum.Value.Model.IsInstanceOf<FolderAlbum>();
			fa.DirectoryPath.Is(this.TestDataDir);
		}

		[Test]
		public void アルバム作成ウィンドウオープン() {
			var vm = new AlbumSelectorViewModel();
			var args = new List<(object sender, InteractionMessageRaisedEventArgs e)>();
			vm.Messenger.Raised += (sender, e) => {
				args.Add((sender, e));
			};
			args.Count.Is(0);
			this.DataBase.Albums.Count().Is(0);
			vm.OpenCreateAlbumWindowCommand.Execute();
			args.Count.Is(1);
			args[0].sender.Is(vm.Messenger);
			var tm = args[0].e.Message.IsInstanceOf<TransitionMessage>();
			tm.Mode.Is(TransitionMode.Normal);
			tm.WindowType.Is(typeof(MediaBox.Views.Album.AlbumEditor));
			tm.TransitionViewModel.IsInstanceOf<AlbumEditorViewModel>();
		}

		[Test]
		public void アルバム編集ウィンドウオープン() {
			var ra2 = new RegisteredAlbum();
			ra2.Create();
			ra2.Title.Value = "title2";
			ra2.AlbumPath.Value = "/pic/fo";
			ra2.ReflectToDataBase();

			var vm = new AlbumSelectorViewModel();
			var args = new List<(object sender, InteractionMessageRaisedEventArgs e)>();
			vm.Messenger.Raised += (sender, e) => {
				args.Add((sender, e));
			};
			args.Count.Is(0);
			vm.OpenEditAlbumWindowCommand.Execute(this.ViewModelFactory.Create(ra2));
			args.Count.Is(1);
			args[0].sender.Is(vm.Messenger);
			var tm = args[0].e.Message.IsInstanceOf<TransitionMessage>();
			tm.Mode.Is(TransitionMode.Normal);
			tm.WindowType.Is(typeof(MediaBox.Views.Album.AlbumEditor));
			var aevm = tm.TransitionViewModel.IsInstanceOf<AlbumEditorViewModel>();
			aevm.Title.Value.Is("title2");
		}
	}
}
