using System.Collections.Generic;
using System.Linq;

using Livet.Messaging;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	internal class AlbumSelectorViewModelTest : ViewModelTestClassBase {

		[Test]
		public void アルバムリスト() {
			var ac = Get.Instance<AlbumContainer>();
			var model = new AlbumSelector("main");
			var vm = new AlbumSelectorViewModel(model);
			var ra1 = new RegisteredAlbum(model);
			ra1.Create();
			ra1.Title.Value = "title1";
			ra1.ReflectToDataBase();
			var ra2 = new RegisteredAlbum(model);
			ra2.Create();
			ra2.Title.Value = "title2";
			ra2.ReflectToDataBase();
			var ra3 = new RegisteredAlbum(model);
			ra3.Create();
			ra3.Title.Value = "title3";
			ra3.ReflectToDataBase();
			vm.AlbumList.Is();
			ac.AddAlbum(ra1.AlbumId.Value);
			vm.AlbumList.Select(x => x.Model).OfType<RegisteredAlbum>().Select(x => x.AlbumId.Value).Is(ra1.AlbumId.Value);
			ac.AddAlbum(ra2.AlbumId.Value);
			vm.AlbumList.Select(x => x.Model).OfType<RegisteredAlbum>().Select(x => x.AlbumId.Value).Is(ra1.AlbumId.Value, ra2.AlbumId.Value);
			ac.AddAlbum(ra3.AlbumId.Value);
			vm.AlbumList.Select(x => x.Model).OfType<RegisteredAlbum>().Select(x => x.AlbumId.Value).Is(ra1.AlbumId.Value, ra2.AlbumId.Value, ra3.AlbumId.Value);
			ac.RemoveAlbum(ra2.AlbumId.Value);
			vm.AlbumList.Select(x => x.Model).OfType<RegisteredAlbum>().Select(x => x.AlbumId.Value).Is(ra1.AlbumId.Value, ra3.AlbumId.Value);
		}

		[Test]
		public void カレントアルバム変更() {
			var ac = Get.Instance<AlbumContainer>();
			var model = new AlbumSelector("main");
			var vm = new AlbumSelectorViewModel(model);
			var ra1 = new RegisteredAlbum(model);
			ra1.Create();
			ra1.AlbumId.Value = 1;
			ra1.Title.Value = "title1";
			ra1.ReflectToDataBase();
			var ra2 = new RegisteredAlbum(model);
			ra2.Create();
			ra2.AlbumId.Value = 2;
			ra2.Title.Value = "title2";
			ra2.ReflectToDataBase();
			var ra3 = new RegisteredAlbum(model);
			ra3.Create();
			ra3.AlbumId.Value = 3;
			ra3.Title.Value = "title3";
			ra3.ReflectToDataBase();
			ac.AddAlbum(ra1.AlbumId.Value);
			ac.AddAlbum(ra2.AlbumId.Value);
			ac.AddAlbum(ra3.AlbumId.Value);
			vm.CurrentAlbum.Value.IsNull();
			vm.SetAlbumToCurrent.Execute(vm.AlbumList[1]);
			vm.CurrentAlbum.Value.Model.Is(vm.AlbumList[1].Model);
		}

		[Test]
		public void カレントフォルダアルバム変更() {
			var model = new AlbumSelector("main");
			var vm = new AlbumSelectorViewModel(model);
			vm.FolderAlbumPath.Value = this.TestDataDir;
			vm.SetFolderAlbumToCurrent.Execute();
			var fa = vm.CurrentAlbum.Value.Model.IsInstanceOf<FolderAlbum>();
			fa.DirectoryPath.Is(this.TestDataDir);
		}

		[Test]
		public void アルバム作成ウィンドウオープン() {
			var model = new AlbumSelector("main");
			var vm = new AlbumSelectorViewModel(model);
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
			var ra2 = new RegisteredAlbum(new AlbumSelector("main"));
			ra2.Create();
			ra2.Title.Value = "title2";
			ra2.AlbumPath.Value = "/pic/fo";
			ra2.ReflectToDataBase();

			var model = new AlbumSelector("main");
			var vm = new AlbumSelectorViewModel(model);
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
