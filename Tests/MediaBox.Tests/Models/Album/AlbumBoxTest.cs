
using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumBoxTest : ModelTestClassBase {
		[Test]
		public void アルバムボックス読み込み() {
			lock (this.Rdb) {
				this.Rdb.AlbumBoxes.AddRange(
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
				});
				this.Rdb.SaveChanges();
			}

			using var albums = new ReactiveCollection<RegisteredAlbum>();
			using var albumBox = new AlbumBox(albums.ToReadOnlyReactiveCollection());
			using var selector = new AlbumSelector("main");

			var zooAlbum = new RegisteredAlbum(selector);
			zooAlbum.Create();
			zooAlbum.AlbumBoxId.Value = 1;
			zooAlbum.Title.Value = "Zoo";
			var dolphinsAlbum = new RegisteredAlbum(selector);
			dolphinsAlbum.Create();
			dolphinsAlbum.AlbumBoxId.Value = 2;
			dolphinsAlbum.Title.Value = "Dolphins";
			var tulipAlbum = new RegisteredAlbum(selector);
			tulipAlbum.Create();
			tulipAlbum.AlbumBoxId.Value = 5;
			tulipAlbum.Title.Value = "tulip";
			albums.AddRange(zooAlbum, dolphinsAlbum, tulipAlbum);


			albumBox.Children.Count.Is(2);
			var animal = albumBox.Children.First();
			animal.Title.Value.Is("動物");
			animal.Children.Count.Is(2);
			animal.Albums.Count().Is(1);
			animal.Albums.Is(zooAlbum);
			var dolphin = animal.Children.First();
			dolphin.Title.Value.Is("いるか");
			dolphin.Children.Is();
			dolphin.Albums.Is(dolphinsAlbum);
			var raccoon = animal.Children.Skip(1).First();
			raccoon.Title.Value.Is("たぬき");
			raccoon.Children.Is();
			raccoon.Albums.Is();
			var plant = albumBox.Children.Skip(1).First();
			plant.Title.Value.Is("植物");
			plant.Children.Count.Is(1);
			plant.Albums.Is();
			var tulip = plant.Children.First();
			tulip.Title.Value.Is("チューリップ");
			tulip.Children.Is();
			tulip.Albums.Is(tulipAlbum);
		}

		[Test]
		public void 子アルバムボックス追加() {
			using var albums = new ReactiveCollection<RegisteredAlbum>();
			using var albumBox = new AlbumBox(albums.ToReadOnlyReactiveCollection());

			albumBox.Children.Count.Is(0);

			lock (this.Rdb) {
				this.Rdb.AlbumBoxes.Count().Is(0);

				albumBox.AddChild("box1");

				this.Rdb.AlbumBoxes.Count().Is(1);
				var boxes =
					this.Rdb
						.AlbumBoxes
						.Include(x => x.Albums)
						.Include(x => x.Children)
						.OrderBy(x => x.AlbumBoxId)
						.ToArray();
				boxes.Length.Is(1);
				var box1 = boxes[0];
				box1.AlbumBoxId.Is(1);
				box1.Albums.Count.Is(0);
				box1.Children.Count.Is(0);
				box1.Name.Is("box1");
				box1.Parent.IsNull();
				box1.ParentAlbumBoxId.IsNull();
			}

			albumBox.Children.Count.Is(1);
			var boxModel1 = albumBox.Children[0];
			boxModel1.AlbumBoxId.Value.Is(1);
			boxModel1.Title.Value.Is("box1");
			boxModel1.Albums.Count.Is(0);
			boxModel1.Children.Count.Is(0);

			lock (this.Rdb) {

				this.Rdb.AlbumBoxes.Count().Is(1);

				boxModel1.AddChild("box2");

				this.Rdb.AlbumBoxes.Count().Is(2);
				var boxes =
					this.Rdb
						.AlbumBoxes
						.Include(x => x.Albums)
						.Include(x => x.Children)
						.OrderBy(x => x.AlbumBoxId)
						.ToArray();
				boxes.Length.Is(2);
				var box2 = boxes[1];
				box2.AlbumBoxId.Is(2);
				box2.Albums.Count.Is(0);
				box2.Children.Count.Is(0);
				box2.Name.Is("box2");
				box2.Parent.IsNotNull();
				box2.ParentAlbumBoxId.Is(1);
			}

			albumBox.Children.Count.Is(1);
			boxModel1.Children.Count.Is(1);
			var boxModel2 = boxModel1.Children[0];
			boxModel2.Title.Value.Is("box2");
			boxModel2.Albums.Count.Is(0);
			boxModel2.Children.Count.Is(0);
		}

		[Test]
		public void 子アルバムボックス削除() {
			using var albums = new ReactiveCollection<RegisteredAlbum>();
			using var albumBox = new AlbumBox(albums.ToReadOnlyReactiveCollection());

			albumBox.Children.Count.Is(0);
			albumBox.AddChild("box1");

			var box1 = albumBox.Children[0];
			box1.AddChild("box1-1");
			var box1C1 = box1.Children[0];

			albumBox.AddChild("box2");
			var box2 = albumBox.Children[1];
			albumBox.AddChild("box3");

			box1.AddChild("box1-2");

			lock (this.Rdb) {
				this.Rdb.AlbumBoxes.Count().Is(5);
				this.Rdb.AlbumBoxes.Select(x => x.AlbumBoxId).OrderBy(x => x).Is(1, 2, 3, 4, 5);
			}

			albumBox.Children.Count.Is(3);
			// 単体削除
			box2.Remove();
			lock (this.Rdb) {
				this.Rdb.AlbumBoxes.Count().Is(4);
				this.Rdb.AlbumBoxes.Select(x => x.AlbumBoxId).OrderBy(x => x).Is(1, 2, 4, 5);
			}
			albumBox.Children.Count.Is(2);

			box1.Children.Count.Is(2);
			// 子単体削除
			box1C1.Remove();
			lock (this.Rdb) {
				this.Rdb.AlbumBoxes.Count().Is(3);
				this.Rdb.AlbumBoxes.Select(x => x.AlbumBoxId).OrderBy(x => x).Is(1, 4, 5);
			}
			box1.Children.Count.Is(1);

			// 親削除 (子も同時に消える)
			box1.Remove();
			lock (this.Rdb) {
				this.Rdb.AlbumBoxes.Count().Is(1);
				this.Rdb.AlbumBoxes.Select(x => x.AlbumBoxId).OrderBy(x => x).Is(4);
			}
			albumBox.Children.Count.Is(1);

			Assert.Throws<InvalidOperationException>(() => {
				albumBox.Remove();
			});
		}

		[Test]
		public void リネーム() {
			using var albums = new ReactiveCollection<RegisteredAlbum>();
			using var albumBox = new AlbumBox(albums.ToReadOnlyReactiveCollection());

			albumBox.Children.Count.Is(0);
			albumBox.AddChild("box1");

			var box1 = albumBox.Children[0];
			box1.AddChild("box1-1");
			var box1C1 = box1.Children[0];

			lock (this.Rdb) {
				var names = this.Rdb.AlbumBoxes.OrderBy(x => x.AlbumBoxId).Select(x => x.Name);
				names.Is("box1", "box1-1");

				box1.Rename("renamed1");
				box1.Title.Value.Is("renamed1");
				names.Is("renamed1", "box1-1");

				box1C1.Rename("renamed1-1");
				box1C1.Title.Value.Is("renamed1-1");
				names.Is("renamed1", "renamed1-1");
			}

			Assert.Throws<InvalidOperationException>(() => {
				albumBox.Rename("box");
			});
		}

		[Test]
		public void アルバムリストの変更追従() {

			using var albums = new ReactiveCollection<RegisteredAlbum>();
			using var albumBox = new AlbumBox(albums.ToReadOnlyReactiveCollection());

			albumBox.Children.Count.Is(0);
			albumBox.AddChild("box1");

			var box1 = albumBox.Children[0];
			box1.AddChild("box1-1");
			var box1C1 = box1.Children[0];

			albumBox.AddChild("box2");
			var box2 = albumBox.Children[1];
			albumBox.AddChild("box3");

			box1.AddChild("box1-2");

			using var selector = new AlbumSelector("main");

			// Zoo追加
			var zooAlbum = new RegisteredAlbum(selector);
			zooAlbum.Create();
			zooAlbum.AlbumBoxId.Value = 1;
			zooAlbum.Title.Value = "Zoo";
			albums.Add(zooAlbum);

			box1.Albums.Is(zooAlbum);

			// Dolphins追加
			var dolphinsAlbum = new RegisteredAlbum(selector);
			dolphinsAlbum.Create();
			dolphinsAlbum.AlbumBoxId.Value = 2;
			dolphinsAlbum.Title.Value = "Dolphins";
			albums.Add(dolphinsAlbum);

			box1C1.Albums.Is(dolphinsAlbum);

			// tulip追加
			var tulipAlbum = new RegisteredAlbum(selector);
			tulipAlbum.Create();
			tulipAlbum.AlbumBoxId.Value = 3;
			tulipAlbum.Title.Value = "tulip";
			albums.Add(tulipAlbum);

			box2.Albums.Is(tulipAlbum);

			// tulipアルバムボックス変更
			tulipAlbum.AlbumBoxId.Value = 2;
			box2.Albums.Is();
			box1C1.Albums.Is(dolphinsAlbum, tulipAlbum);

			// Dolphins削除
			albums.Remove(dolphinsAlbum);

			box1C1.Albums.Is(tulipAlbum);
		}
	}
}
