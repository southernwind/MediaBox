using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;

namespace SandBeige.MediaBox.Tests.Models.Album {
	internal class AlbumContainerTest : ModelTestClassBase {
		[Test]
		public void アルバムリスト読み込み() {
			using (var ra1 = new RegisteredAlbum())
			using (var ra2 = new RegisteredAlbum())
			using (var ra3 = new RegisteredAlbum()) {
				ra1.Create();
				ra1.Title.Value = "ra1";
				ra1.ReflectToDataBase();
				ra2.Create();
				ra2.Title.Value = "ra2";
				ra2.ReflectToDataBase();
				ra3.Create();
				ra3.Title.Value = "ra3";
				ra3.ReflectToDataBase();
			}

			var ac = new AlbumContainer();
			ac.AlbumList.Select(x => x.Title.Value).Is("ra1", "ra2", "ra3");
		}

		[Test]
		public void アルバムリスト追加削除() {
			var ac = new AlbumContainer();
			using var ra1 = new RegisteredAlbum();
			ra1.Create();
			ra1.Title.Value = "ra1";
			ra1.ReflectToDataBase();

			using var ra2 = new RegisteredAlbum();
			ra2.Create();
			ra2.Title.Value = "ra2";
			ra2.ReflectToDataBase();

			using var ra3 = new RegisteredAlbum();
			ra3.Create();
			ra3.Title.Value = "ra3";
			ra3.ReflectToDataBase();

			ac.AlbumList.Select(x => x.Title.Value).Is();
			ac.AddAlbum(ra1);
			ac.AlbumList.Select(x => x.Title.Value).Is("ra1");
			ac.AddAlbum(ra2);
			ac.AlbumList.Select(x => x.Title.Value).Is("ra1", "ra2");
			ac.RemoveAlbum(ra1);
			ac.AlbumList.Select(x => x.Title.Value).Is("ra2");

		}
	}
}
