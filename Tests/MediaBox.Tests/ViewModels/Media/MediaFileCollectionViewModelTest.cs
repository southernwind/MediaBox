using Livet;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	internal class MediaFileCollectionViewModelTest : ViewModelTestClassBase {
		[Test]
		public void モデル() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var model = new MediaFileCollection(osc);
			using var vm = new MediaFileCollectionViewModelImpl(model);
			vm.Model.Is(model);
		}

		[Test]
		public void 件数() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var model = new MediaFileCollection(osc);
			model.Count.Value = 5;
			using var vm = new MediaFileCollectionViewModelImpl(model);
			vm.Count.Value.Is(5);
			model.Count.Value = 9;
			vm.Count.Value.Is(9);
		}

		[Test]
		public void メディアファイルリスト() {
			using var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			using var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			using var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			using var image4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);

			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var model = new MediaFileCollection(osc);
			model.Items.AddRange(image1, image2, image3);
			using var vm = new MediaFileCollectionViewModelImpl(model);
			vm.Items.Is(
				this.ViewModelFactory.Create(image1),
				this.ViewModelFactory.Create(image2),
				this.ViewModelFactory.Create(image3)
			);

			model.Items.Add(image4);
			vm.Items.Is(
				this.ViewModelFactory.Create(image1),
				this.ViewModelFactory.Create(image2),
				this.ViewModelFactory.Create(image3),
				this.ViewModelFactory.Create(image4)
			);
		}

		private class MediaFileCollectionViewModelImpl : MediaFileCollectionViewModel<MediaFileCollection> {
			public MediaFileCollectionViewModelImpl(MediaFileCollection mediaFileCollection) : base(mediaFileCollection) {
			}
		}
	}
}
