using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.History;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.Tests.Models.Album.History {
	internal class AlbumHistoryManagerTest : ModelTestClassBase {

		[Test]
		public void 追加() {
			var statesMock = ModelMockCreator.CreateStatesMock();
			statesMock.Object.AlbumStates.AlbumHistory.Count.Should().Be(0);

			using var ahm = new AlbumHistoryManager(statesMock.Object);
			var album1 = ModelMockCreator.CreateAlbumObjectMock();
			var album2 = ModelMockCreator.CreateAlbumObjectMock();
			var album3 = ModelMockCreator.CreateAlbumObjectMock();
			var album4 = ModelMockCreator.CreateAlbumObjectMock();
			ahm.Add("title1", album1.Object);
			ahm.Add("title2", album2.Object);
			ahm.Add("title3", album3.Object);
			// album4は同一タイトルなので追加されない
			ahm.Add("title3", album4.Object);

			statesMock.Object.AlbumStates.AlbumHistory.Count.Should().Be(3);
			statesMock.Object.AlbumStates.AlbumHistory[0].Title.Should().Be("title3");
			statesMock.Object.AlbumStates.AlbumHistory[1].Title.Should().Be("title2");
			statesMock.Object.AlbumStates.AlbumHistory[2].Title.Should().Be("title1");
		}

		[Test]
		public void 履歴保存上限() {
			var statesMock = ModelMockCreator.CreateStatesMock();
			using var ahm = new AlbumHistoryManager(statesMock.Object);

			foreach (var id in Enumerable.Range(1, 11)) {
				var eao = ModelMockCreator.CreateEditableAlbumObjectMock();
				eao.Setup(x => x.AlbumId).Returns(id);
				ahm.Add($"album{id}", eao.Object);
			}

			statesMock.Object.AlbumStates.AlbumHistory.Count.Should().Be(10);
			statesMock.Object.AlbumStates.AlbumHistory[0].Title.Should().Be("album11");
			statesMock.Object.AlbumStates.AlbumHistory[1].Title.Should().Be("album10");
			statesMock.Object.AlbumStates.AlbumHistory[2].Title.Should().Be("album9");
			statesMock.Object.AlbumStates.AlbumHistory[3].Title.Should().Be("album8");
			statesMock.Object.AlbumStates.AlbumHistory[4].Title.Should().Be("album7");
			statesMock.Object.AlbumStates.AlbumHistory[5].Title.Should().Be("album6");
			statesMock.Object.AlbumStates.AlbumHistory[6].Title.Should().Be("album5");
			statesMock.Object.AlbumStates.AlbumHistory[7].Title.Should().Be("album4");
			statesMock.Object.AlbumStates.AlbumHistory[8].Title.Should().Be("album3");
			statesMock.Object.AlbumStates.AlbumHistory[9].Title.Should().Be("album2");
		}
	}
}
