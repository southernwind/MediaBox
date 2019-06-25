
using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	internal class AlbumContainerTest : ModelTestClassBase {
		[Test]
		public void アルバムリスト読み込み() {
			using var selector = new AlbumSelector("main");
			using (var ra1 = new RegisteredAlbum(selector))
			using (var ra2 = new RegisteredAlbum(selector))
			using (var ra3 = new RegisteredAlbum(selector)) {
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

			using var ac = new AlbumContainer();
			ac.AlbumList.Is(1, 2, 3);
		}

		[Test]
		public void アルバムリスト追加削除() {
			using var ac = Get.Instance<AlbumContainer>();
			using var selector = new AlbumSelector("main");
			using var ra1 = new RegisteredAlbum(selector);
			ra1.Create();
			ra1.Title.Value = "ra1";
			ra1.ReflectToDataBase();

			using var ra2 = new RegisteredAlbum(selector);
			ra2.Create();
			ra2.Title.Value = "ra2";
			ra2.ReflectToDataBase();

			using var ra3 = new RegisteredAlbum(selector);
			ra3.Create();
			ra3.Title.Value = "ra3";
			ra3.ReflectToDataBase();

			ac.AlbumList.Is();
			ac.AddAlbum(ra1.AlbumId.Value);
			ac.AlbumList.Is(1);
			ac.AddAlbum(ra2.AlbumId.Value);
			ac.AlbumList.Is(1, 2);
			ac.RemoveAlbum(ra1.AlbumId.Value);
			ac.AlbumList.Is(2);

		}
	}
}
