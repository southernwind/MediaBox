using Livet.StatefulModel;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.Tests.Implements;

namespace SandBeige.MediaBox.Tests.Models.Album.Viewer {
	internal class ListViewerModelTest : ModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = new AlbumModelForTest(osc, selector);
			using var model = new MapViewerModel(album);
		}
	}
}
