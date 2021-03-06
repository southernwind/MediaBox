using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Box;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.Tests.Models.Album {
	internal class AlbumBoxSelectorTest : ModelTestClassBase {
		[Test]
		public void Dispose追従() {
			var creator = new DbContextMockCreator();
			var albumLoaderFactoryMock = ModelMockCreator.CreateAlbumLoaderFactoryMock();
			var albumBoxSelector = new AlbumBoxSelector(creator.Mock.Object, albumLoaderFactoryMock.Object);
			var box = albumBoxSelector.Shelf.Value;
			box.Should().NotBeNull();

			box.DisposeState.Should().Be(DisposeState.NotDisposed);
			albumBoxSelector.Dispose();
			box.DisposeState.Should().Be(DisposeState.Disposed);
		}
	}
}
