
using System.Collections.Generic;

using Livet.Messaging;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.ViewModels.Media;
using SandBeige.MediaBox.Views.Map;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	internal class MediaFileInformationViewModelTest : ViewModelTestClassBase {
		[Test]
		public void 件数() {
			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var image4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);

			var model = new MediaFileInformation();
			model.Files.Value = new[] { image1, image3 };
			var vm = new MediaFileInformationViewModel(model);
			vm.Files.Value.Is(
				this.ViewModelFactory.Create(image1),
				this.ViewModelFactory.Create(image3)
			);
			vm.FilesCount.Value.Is(2);

			model.Files.Value = new[] { image1, image2, image4 };
			vm.Files.Value.Is(
				this.ViewModelFactory.Create(image1),
				this.ViewModelFactory.Create(image2),
				this.ViewModelFactory.Create(image4)
			);
			vm.FilesCount.Value.Is(3);
		}

		[Test]
		public void Gps設定ウィンドウオープン() {
			var model = new MediaFileInformation();
			var vm = new MediaFileInformationViewModel(model);
			var args = new List<(object sender, InteractionMessageRaisedEventArgs e)>();
			vm.Messenger.Raised += (sender, e) => {
				args.Add((sender, e));
			};
			args.Count.Is(0);
			vm.OpenGpsSelectorWindowCommand.Execute();
			args.Count.Is(1);
			args[0].sender.Is(vm.Messenger);
			var tm = args[0].e.Message.IsInstanceOf<TransitionMessage>();
			tm.Mode.Is(TransitionMode.Modal);
			tm.WindowType.Is(typeof(GpsSelectorWindow));
			tm.TransitionViewModel.IsInstanceOf<GpsSelectorViewModel>();
		}
	}
}
