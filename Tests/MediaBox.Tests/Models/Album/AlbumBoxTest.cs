
using System;
using System.Linq;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Box;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumBoxTest : ModelTestClassBase {
		[Test]
		public void アルバムボックス読み込み() {
			var creator = new DbContextMockCreator();
			creator.SetData(
				new DataBase.Tables.AlbumBox {
					AlbumBoxId = 1,
					Name = "動物",
					ParentAlbumBoxId = null
				}, new DataBase.Tables.AlbumBox {
					AlbumBoxId = 2,
					Name = "いるか",
					ParentAlbumBoxId = 1
				}, new DataBase.Tables.AlbumBox {
					AlbumBoxId = 3,
					Name = "たぬき",
					ParentAlbumBoxId = 1
				}, new DataBase.Tables.AlbumBox {
					AlbumBoxId = 4,
					Name = "植物",
					ParentAlbumBoxId = null
				}, new DataBase.Tables.AlbumBox {
					AlbumBoxId = 5,
					Name = "チューリップ",
					ParentAlbumBoxId = 4
				}
			);


			using var albums = new ReactiveCollection<RegisteredAlbumObject>();
			using var albumBox = new AlbumBox(albums.ToReadOnlyReactiveCollection(), creator.Mock.Object);

			creator.SetData(
				new DataBase.Tables.Album {
					AlbumId = 1,
					AlbumBoxId = 1,
					Title = "Zoo"
				},
				new DataBase.Tables.Album {
					AlbumId = 2,
					AlbumBoxId = 2,
					Title = "Dolphins"
				},
				new DataBase.Tables.Album {
					AlbumId = 4,
					AlbumBoxId = 5,
					Title = "tulip"
				}
			);
			albums.AddRange(
				new RegisteredAlbumObject { AlbumId = 1 },
				new RegisteredAlbumObject { AlbumId = 2 },
				new RegisteredAlbumObject { AlbumId = 4 }
			);

			albumBox.Children.Count.Should().Be(2);
			var animal = albumBox.Children.First();
			animal.Title.Value.Should().Be("動物");
			animal.Children.Count.Should().Be(2);
			animal.Albums.Count().Should().Be(1);
			animal.Albums.First().Title.Value.Should().Be("Zoo");
			var dolphin = animal.Children.First();
			dolphin.Title.Value.Should().Be("いるか");
			dolphin.Children.Should().BeEmpty();
			dolphin.Albums.First().Title.Value.Should().Be("Dolphins");
			var raccoon = animal.Children.Skip(1).First();
			raccoon.Title.Value.Should().Be("たぬき");
			raccoon.Children.Should().BeEmpty();
			raccoon.Albums.Should().BeEmpty();
			var plant = albumBox.Children.Skip(1).First();
			plant.Title.Value.Should().Be("植物");
			plant.Children.Count.Should().Be(1);
			plant.Albums.Should().BeEmpty();
			var tulip = plant.Children.First();
			tulip.Title.Value.Should().Be("チューリップ");
			tulip.Children.Should().BeEmpty();
			tulip.Albums.First().Title.Value.Should().Be("tulip");
		}

		[Test]
		public void 子アルバムボックス追加() {
			var creator = new DbContextMockCreator();
			using var albums = new ReactiveCollection<RegisteredAlbumObject>();
			using var albumBox = new AlbumBox(albums.ToReadOnlyReactiveCollection(), creator.Mock.Object);

			albumBox.Children.Count.Should().Be(0);

			creator.Mock.Object.AlbumBoxes.Count().Should().Be(0);

			albumBox.AddChild("box1");

			creator.Mock.Object.AlbumBoxes.Count().Should().Be(1);
			var boxes =
				creator.Mock.Object
					.AlbumBoxes
					.Include(x => x.Albums)
					.Include(x => x.Children)
					.OrderBy(x => x.AlbumBoxId)
					.ToArray();
			boxes.Length.Should().Be(1);
			var box1 = boxes[0];
			box1.AlbumBoxId.Should().Be(1);
			box1.Albums.Count.Should().Be(0);
			box1.Children.Count.Should().Be(0);
			box1.Name.Should().Be("box1");
			box1.Parent.Should().BeNull();
			box1.ParentAlbumBoxId.Should().BeNull();

			albumBox.Children.Count.Should().Be(1);
			var boxModel1 = albumBox.Children[0];
			boxModel1.AlbumBoxId.Value.Should().Be(1);
			boxModel1.Title.Value.Should().Be("box1");
			boxModel1.Albums.Count.Should().Be(0);
			boxModel1.Children.Count.Should().Be(0);

			creator.Mock.Object.AlbumBoxes.Count().Should().Be(1);

			boxModel1.AddChild("box2");

			creator.Mock.Object.AlbumBoxes.Count().Should().Be(2);
			var boxes2 =
				creator.Mock.Object
					.AlbumBoxes
					.Include(x => x.Albums)
					.Include(x => x.Children)
					.OrderBy(x => x.AlbumBoxId)
					.ToArray();
			boxes2.Length.Should().Be(2);
			var box2 = boxes2[1];
			box2.AlbumBoxId.Should().Be(2);
			box2.Albums.Count.Should().Be(0);
			box2.Children.Count.Should().Be(0);
			box2.Name.Should().Be("box2");
			box2.Parent.Should().NotBeNull();
			box2.ParentAlbumBoxId.Should().Be(1);

			albumBox.Children.Count.Should().Be(1);
			boxModel1.Children.Count.Should().Be(1);
			var boxModel2 = boxModel1.Children[0];
			boxModel2.Title.Value.Should().Be("box2");
			boxModel2.Albums.Count.Should().Be(0);
			boxModel2.Children.Count.Should().Be(0);
		}

		[Test]
		public void 子アルバムボックス削除() {
			var creator = new DbContextMockCreator();
			using var albums = new ReactiveCollection<RegisteredAlbumObject>();
			using var albumBox = new AlbumBox(albums.ToReadOnlyReactiveCollection(), creator.Mock.Object);

			albumBox.Children.Count.Should().Be(0);
			albumBox.AddChild("box1");

			var box1 = albumBox.Children[0];
			box1.AddChild("box1-1");
			var box1C1 = box1.Children[0];

			albumBox.AddChild("box2");
			var box2 = albumBox.Children[1];
			albumBox.AddChild("box3");

			box1.AddChild("box1-2");

			creator.Mock.Object.AlbumBoxes.Count().Should().Be(5);
			creator.Mock.Object.AlbumBoxes.Select(x => x.AlbumBoxId).OrderBy(x => x).Should().Equal(new[] { 1, 2, 3, 4, 5 });

			albumBox.Children.Count.Should().Be(3);
			// 単体削除
			box2.Remove();
			creator.Mock.Object.AlbumBoxes.Count().Should().Be(4);
			creator.Mock.Object.AlbumBoxes.Select(x => x.AlbumBoxId).OrderBy(x => x).Should().Equal(new[] { 1, 2, 4, 5 });
			albumBox.Children.Count.Should().Be(2);

			box1.Children.Count.Should().Be(2);
			// 子単体削除
			box1C1.Remove();
			creator.Mock.Object.AlbumBoxes.Count().Should().Be(3);
			creator.Mock.Object.AlbumBoxes.Select(x => x.AlbumBoxId).OrderBy(x => x).Should().Equal(new[] { 1, 4, 5 });
			box1.Children.Count.Should().Be(1);

			// 親削除 (子も同時に消える)
			box1.Remove();
			creator.Mock.Object.AlbumBoxes.Count().Should().Be(1);
			creator.Mock.Object.AlbumBoxes.Select(x => x.AlbumBoxId).OrderBy(x => x).Should().Equal(new[] { 4 });
			albumBox.Children.Count.Should().Be(1);

			Action act = () => {
				albumBox.Remove();
			};
			act.Should().ThrowExactly<InvalidOperationException>();
		}

		[Test]
		public void リネーム() {
			var creator = new DbContextMockCreator();
			using var albums = new ReactiveCollection<RegisteredAlbumObject>();
			using var albumBox = new AlbumBox(albums.ToReadOnlyReactiveCollection(), creator.Mock.Object);

			albumBox.Children.Count.Should().Be(0);
			albumBox.AddChild("box1");

			var box1 = albumBox.Children[0];
			box1.AddChild("box1-1");
			var box1C1 = box1.Children[0];

			var names = creator.Mock.Object.AlbumBoxes.OrderBy(x => x.AlbumBoxId).Select(x => x.Name);
			names.Should().Equal(new[] { "box1", "box1-1" });

			box1.Rename("renamed1");
			box1.Title.Value.Should().Be("renamed1");
			names.Should().Equal(new[] { "renamed1", "box1-1" });

			box1C1.Rename("renamed1-1");
			box1C1.Title.Value.Should().Be("renamed1-1");
			names.Should().Equal(new[] { "renamed1", "renamed1-1" });

			Action act = () => {
				albumBox.Rename("box");
			};
			act.Should().ThrowExactly<InvalidOperationException>();
		}

		[Test]
		public void アルバムリストの変更追従() {
			var creator = new DbContextMockCreator();
			using var albums = new ReactiveCollection<RegisteredAlbumObject>();
			using var albumBox = new AlbumBox(albums.ToReadOnlyReactiveCollection(), creator.Mock.Object);

			albumBox.Children.Count.Should().Be(0);
			albumBox.AddChild("box1");

			var box1 = albumBox.Children[0];
			box1.AddChild("box1-1");
			var box1C1 = box1.Children[0];

			albumBox.AddChild("box2");
			var box2 = albumBox.Children[1];
			albumBox.AddChild("box3");

			box1.AddChild("box1-2");

			// Zoo追加
			creator.SetData(new DataBase.Tables.Album { AlbumId = 1, Title = "Zoo", AlbumBoxId = 1 });
			albums.Add(new RegisteredAlbumObject { AlbumId = 1 });

			box1.Albums.Count.Should().Be(1);
			box1.Albums.First().Title.Value.Should().Be("Zoo");

			// Dolphins追加
			creator.SetData(new DataBase.Tables.Album { AlbumId = 2, Title = "Dolphins", AlbumBoxId = 2 });
			albums.Add(new RegisteredAlbumObject { AlbumId = 2 });

			box1C1.Albums.Count.Should().Be(1);
			box1C1.Albums.First().Title.Value.Should().Be("Dolphins");

			// tulip追加
			creator.SetData(new DataBase.Tables.Album { AlbumId = 3, Title = "tulip", AlbumBoxId = 3 });
			albums.Add(new RegisteredAlbumObject { AlbumId = 3 });

			box2.Albums.Count.Should().Be(1);
			box2.Albums.First().Title.Value.Should().Be("tulip");

			// tulipアルバムボックス変更
			box2.Albums.First().AlbumBoxId.Value = 2;
			box2.Albums.Should().BeEmpty();
			box1C1.Albums.Select(x => x.Title.Value).Should().Equal(new[] { "Dolphins", "tulip" });

			// Dolphins削除
			albums.Remove(albums.First(x => x.AlbumId == 2));

			box1C1.Albums.Select(x => x.Title.Value).Should().Equal(new[] { "tulip" });
		}
	}
}
