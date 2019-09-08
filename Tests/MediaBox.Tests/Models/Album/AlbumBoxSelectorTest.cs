using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album;

namespace SandBeige.MediaBox.Tests.Models.Album {
	internal class AlbumBoxSelectorTest : ModelTestClassBase {
		[Test]
		public void Dispose追従() {
			var albumBoxSelector = new AlbumBoxSelector();
			var box = albumBoxSelector.Shelf.Value;
			box.IsNotNull();

			box.DisposeState.Is(DisposeState.NotDisposed);
			albumBoxSelector.Dispose();
			box.DisposeState.Is(DisposeState.Disposed);
		}
	}
}
