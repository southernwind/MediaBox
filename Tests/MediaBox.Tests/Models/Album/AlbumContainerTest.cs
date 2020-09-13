
using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.Tests.Models.Album {
	internal class AlbumContainerTest : ModelTestClassBase {
		[Test]
		public void アルバムリスト読み込み() {
			var creator = new DbContextMockCreator();
			creator.SetData(
				new DataBase.Tables.Album {
					AlbumId = 1
				},
				new DataBase.Tables.Album {
					AlbumId = 3
				},
				new DataBase.Tables.Album {
					AlbumId = 7
				}
			);

			using var ac = new AlbumContainer(creator.Mock.Object);
			ac.AlbumList.Should().Equal(1, 3, 7);
		}

		[Test]
		public void アルバムリスト追加削除() {
			var creator = new DbContextMockCreator();
			creator.SetData(
				new DataBase.Tables.Album {
					AlbumId = 1
				}
			);

			using var ac = new AlbumContainer(creator.Mock.Object);
			creator.SetData(
				new DataBase.Tables.Album {
					AlbumId = 6
				},
				new DataBase.Tables.Album {
					AlbumId = 2
				}
			);
			ac.AddAlbum(6);
			ac.AddAlbum(2);
			ac.AlbumList.Should().Equal(1, 6, 2);
			creator.Mock.Object.Albums.Select(x => x.AlbumId).Should().BeEquivalentTo(1, 2, 6);
			ac.RemoveAlbum(new RegisteredAlbumObject { AlbumId = 6 });
			creator.Mock.Object.Albums.Select(x => x.AlbumId).Should().BeEquivalentTo(1, 2);
			ac.AlbumList.Should().Equal(1, 2);


		}

		[Test]
		public void アルバム更新通知() {
			var creator = new DbContextMockCreator();
			creator.SetData(
				new DataBase.Tables.Album {
					AlbumId = 1
				}
			);

			using var ac = new AlbumContainer(creator.Mock.Object);
			var args = new List<int>();
			ac.AlbumUpdated.Subscribe(args.Add);

			args.Should().BeEmpty();
			ac.OnAlbumUpdated(3);
			args.Should().Equal(3);
			ac.OnAlbumUpdated(5);
			args.Should().Equal(3, 5);
			ac.OnAlbumUpdated(1);
			args.Should().Equal(3, 5, 1);
		}
	}
}
