
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumBoxTest : ModelTestClassBase {
		[Test]
		public void 階層構造() {
			using var ac = Get.Instance<AlbumContainer>();
			lock (this.DataBase) {
				this.DataBase.AlbumBoxes.AddRange(new[]{
					new DataBase.Tables.AlbumBox() {
						AlbumBoxId=1,
						Name="動物",
						ParentAlbumBoxId=null
					},
					new DataBase.Tables.AlbumBox() {
						AlbumBoxId=2,
						Name="いるか",
						ParentAlbumBoxId=1
					},
					new DataBase.Tables.AlbumBox() {
						AlbumBoxId=3,
						Name="たぬき",
						ParentAlbumBoxId=1
					},
					new DataBase.Tables.AlbumBox() {
						AlbumBoxId=4,
						Name="植物",
						ParentAlbumBoxId=null
					},
					new DataBase.Tables.AlbumBox() {
						AlbumBoxId=5,
						Name="チューリップ",
						ParentAlbumBoxId=4
					},
				});
			}
			this.DataBase.SaveChanges();

			using var selector = new AlbumSelector("main");
			var shelf = selector.Shelf.Value;
			var zooAlbum = new RegisteredAlbum(selector);
			zooAlbum.Create();
			zooAlbum.AlbumBoxId.Value = 1;
			zooAlbum.Title.Value = "Zoo";
			zooAlbum.ReflectToDataBase();
			var dolphinsAlbum = new RegisteredAlbum(selector);
			dolphinsAlbum.Create();
			dolphinsAlbum.AlbumBoxId.Value = 2;
			dolphinsAlbum.Title.Value = "Dolphins";
			dolphinsAlbum.ReflectToDataBase();
			var tulipAlbum = new RegisteredAlbum(selector);
			tulipAlbum.Create();
			tulipAlbum.AlbumBoxId.Value = 5;
			tulipAlbum.Title.Value = "tulip";
			tulipAlbum.ReflectToDataBase();

			ac.AddAlbum(zooAlbum.AlbumId.Value);
			zooAlbum = selector.AlbumList.Single(x => x.AlbumId.Value == zooAlbum.AlbumId.Value);
			ac.AddAlbum(dolphinsAlbum.AlbumId.Value);
			dolphinsAlbum = selector.AlbumList.Single(x => x.AlbumId.Value == dolphinsAlbum.AlbumId.Value);
			ac.AddAlbum(tulipAlbum.AlbumId.Value);
			tulipAlbum = selector.AlbumList.Single(x => x.AlbumId.Value == tulipAlbum.AlbumId.Value);

			// 3つアルバム追加した時点での確認
			selector.AlbumList.Is(zooAlbum, dolphinsAlbum, tulipAlbum);
			shelf.Children.Count().Is(2);
			var animal = shelf.Children.First();
			animal.Title.Value.Is("動物");
			animal.Children.Count().Is(2);
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
			var plant = shelf.Children.Skip(1).First();
			plant.Title.Value.Is("植物");
			plant.Children.Count().Is(1);
			plant.Albums.Is();
			var tulip = plant.Children.First();
			tulip.Title.Value.Is("チューリップ");
			tulip.Children.Is();
			tulip.Albums.Is(tulipAlbum);
		}
	}
}
