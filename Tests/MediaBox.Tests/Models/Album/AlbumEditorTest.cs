using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Editor;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumEditorTest : ModelTestClassBase {
		[Test]
		public void アルバム作成保存() {
			var albumContainerMock = ModelMockCreator.CreateAlbumContainerMock();
			var albumSelectorProviderMock = ModelMockCreator.CreateAlbumSelectorProviderMock();
			var albumSelectorMock = ModelMockCreator.CreateAlbumSelectorMock();
			albumSelectorProviderMock.Setup(x => x.Create("editor")).Returns(albumSelectorMock.Object);
			var creator = new DbContextMockCreator();
			var albumModel = ModelMockCreator.CreateAlbumForEditorModelMock();
			using var editor = new AlbumEditor(albumContainerMock.Object, albumSelectorProviderMock.Object, creator.Mock.Object, albumModel.Object);
			editor.Title.Value = "album";
			editor.Save();

			albumModel.Verify(x => x.Create(It.IsAny<RegisteredAlbumObject>(), albumSelectorMock.Object.FilterSetter, albumSelectorMock.Object.SortSetter), Times.Once());
			albumModel.Verify(x => x.ReflectToDataBase(), Times.Once());
			albumModel.Object.Title.Value.Should().Be("album");
		}


		[Test]
		public void アルバム編集() {
			var albumContainerMock = ModelMockCreator.CreateAlbumContainerMock();
			var albumSelectorProviderMock = ModelMockCreator.CreateAlbumSelectorProviderMock();
			var albumSelectorMock = ModelMockCreator.CreateAlbumSelectorMock();
			albumSelectorProviderMock.Setup(x => x.Create("editor")).Returns(albumSelectorMock.Object);
			var creator = new DbContextMockCreator();
			var albumModel = ModelMockCreator.CreateAlbumForEditorModelMock();
			using var editor = new AlbumEditor(albumContainerMock.Object, albumSelectorProviderMock.Object, creator.Mock.Object, albumModel.Object);
			var registeredAlbumObject = new RegisteredAlbumObject();
			editor.EditAlbum(registeredAlbumObject);

			albumModel.Verify(x => x.SetAlbumObject(registeredAlbumObject, albumSelectorMock.Object.FilterSetter, albumSelectorMock.Object.SortSetter));
		}

		[Test]
		public async Task 読み込み() {
			var albumContainerMock = ModelMockCreator.CreateAlbumContainerMock();
			var albumSelectorProviderMock = ModelMockCreator.CreateAlbumSelectorProviderMock();
			var creator = new DbContextMockCreator();
			var albumModel = ModelMockCreator.CreateAlbumForEditorModelMock();
			albumModel.Setup(x => x.AlbumBoxId.Value).Returns(5);
			albumModel.Setup(x => x.Title.Value).Returns("アルバムタイトル");
			albumModel.Setup(x => x.Directories).Returns(new ReactiveCollection<string>() { "dir1", "dir3" });
			using var editor = new AlbumEditor(albumContainerMock.Object, albumSelectorProviderMock.Object, creator.Mock.Object, albumModel.Object);
			await editor.Load();
			editor.AlbumBoxId.Value.Should().Be(5);
			editor.Title.Value.Should().Be("アルバムタイトル");
			editor.MonitoringDirectories.Should().Equal("dir1", "dir3");
		}

		[Test]
		public void アルバム変更通知() {
			var albumContainerMock = ModelMockCreator.CreateAlbumContainerMock();
			var albumSelectorProviderMock = ModelMockCreator.CreateAlbumSelectorProviderMock();
			var creator = new DbContextMockCreator();
			var albumModel = ModelMockCreator.CreateAlbumForEditorModelMock();
			albumModel.Setup(x => x.AlbumId.Value).Returns(8);
			albumModel.Setup(x => x.AlbumBoxId).Returns(new ReactiveProperty<int?>());
			albumModel.Setup(x => x.Title).Returns(new ReactiveProperty<string>());
			albumModel.Setup(x => x.Directories).Returns(new ReactiveCollection<string>());
			using var editor = new AlbumEditor(albumContainerMock.Object, albumSelectorProviderMock.Object, creator.Mock.Object, albumModel.Object);

			editor.Save();
			albumContainerMock.Verify(x => x.OnAlbumUpdated(8), Times.Once());
		}
	}
}
