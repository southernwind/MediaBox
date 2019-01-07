
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumBoxTest : TestClassBase {
		[Test]
		public void Update() {
			var ac = Get.Instance<AlbumContainer>();
			var shelf = ac.Shelf.Value;
			var iphoneOther = Get.Instance<RegisteredAlbum>();
			iphoneOther.AlbumPath.Value = "/iphone";
			iphoneOther.Title.Value = "other";
			var iphonePictureSea = Get.Instance<RegisteredAlbum>();
			iphonePictureSea.AlbumPath.Value = "/iphone/picture";
			iphonePictureSea.Title.Value = "sea";
			var iphoneGameMspoke = Get.Instance<RegisteredAlbum>();
			iphoneGameMspoke.AlbumPath.Value = "/iphone/game";
			iphoneGameMspoke.Title.Value = "mspoke";
			var iphonePictureRiver = Get.Instance<RegisteredAlbum>();
			iphonePictureRiver.AlbumPath.Value = "/iphone/picture";
			iphonePictureRiver.Title.Value = "river";
			var androidPictureSea = Get.Instance<RegisteredAlbum>();
			androidPictureSea.AlbumPath.Value = "/android/picture";
			androidPictureSea.Title.Value = "sea";
			var iphonePictureMountain = Get.Instance<RegisteredAlbum>();
			iphonePictureMountain.AlbumPath.Value = "/iphone/picture";
			iphonePictureMountain.Title.Value = "mountain";
			var androidGameFF7 = Get.Instance<RegisteredAlbum>();
			androidGameFF7.AlbumPath.Value = "/android/game";
			androidGameFF7.Title.Value = "FF7";
			ac.AddAlbum(iphoneOther);
			ac.AddAlbum(iphonePictureSea);
			ac.AddAlbum(iphoneGameMspoke);
			ac.AddAlbum(iphonePictureRiver);
			ac.AddAlbum(androidPictureSea);

			// 3つアルバム追加した時点での確認
			ac.AlbumList.Is(iphoneOther, iphonePictureSea, iphoneGameMspoke, iphonePictureRiver, androidPictureSea);
			shelf.Title.Value.Is("root");
			shelf.Children.Count().Is(2);
			var iphone = shelf.Children.First();
			iphone.Title.Value.Is("iphone");
			iphone.Children.Count().Is(2);
			iphone.Albums.Count().Is(1);
			iphone.Albums.Is(iphoneOther);
			var iphonePicture = iphone.Children.First();
			iphonePicture.Title.Value.Is("picture");
			iphonePicture.Children.Is();
			iphonePicture.Albums.Is(iphonePictureSea, iphonePictureRiver);
			var iphoneGame = iphone.Children.Skip(1).First();
			iphoneGame.Title.Value.Is("game");
			iphoneGame.Children.Is();
			iphoneGame.Albums.Is(iphoneGameMspoke);
			var android = shelf.Children.Skip(1).First();
			android.Title.Value.Is("android");
			android.Children.Count().Is(1);
			android.Albums.Is();
			var androidPicture = android.Children.First();
			androidPicture.Title.Value.Is("picture");
			androidPicture.Children.Is();
			androidPicture.Albums.Is(androidPictureSea);

			// あとから追加
			ac.AddAlbum(iphonePictureMountain);
			ac.AddAlbum(androidGameFF7);
			// 参照は変わらず
			shelf.Title.Value.Is("root");
			shelf.Children.Count().Is(2);
			shelf.Children.First().Is(iphone);
			iphone.Title.Value.Is("iphone");
			iphone.Children.Count().Is(2);
			iphone.Albums.Count().Is(1);
			iphone.Albums.Is(iphoneOther);
			iphone.Children.First().Is(iphonePicture);
			iphonePicture.Title.Value.Is("picture");
			iphonePicture.Children.Is();
			iphonePicture.Albums.Is(iphonePictureSea, iphonePictureRiver, iphonePictureMountain); // 変化
			iphone.Children.Skip(1).First().Is(iphoneGame);
			iphoneGame.Title.Value.Is("game");
			iphoneGame.Children.Is();
			iphoneGame.Albums.Is(iphoneGameMspoke);
			shelf.Children.Skip(1).First().Is(android);
			android.Title.Value.Is("android");
			android.Children.Count().Is(2); // 変化
			android.Albums.Is();
			android.Children.First().Is(androidPicture);
			androidPicture.Title.Value.Is("picture");
			androidPicture.Children.Is();
			androidPicture.Albums.Is(androidPictureSea);
			var androidGame = android.Children.Skip(1).First(); // 追加
			androidGame.Title.Value.Is("game");
			androidGame.Children.Is();
			androidGame.Albums.Is(androidGameFF7);

			// 削除
			ac.RemoveAlbum(iphonePictureSea);
			ac.RemoveAlbum(iphoneGameMspoke);
			ac.RemoveAlbum(androidPictureSea);
			// 参照は変わらず
			shelf.Title.Value.Is("root");
			shelf.Children.Count().Is(2);
			shelf.Children.First().Is(iphone);
			iphone.Title.Value.Is("iphone");
			iphone.Children.Count().Is(1); // 変化
			iphone.Albums.Count().Is(1);
			iphone.Albums.Is(iphoneOther);
			iphone.Children.First().Is(iphonePicture);
			iphonePicture.Title.Value.Is("picture");
			iphonePicture.Children.Is();
			iphonePicture.Albums.Is(iphonePictureRiver, iphonePictureMountain); // 変化
			shelf.Children.Skip(1).First().Is(android);
			android.Title.Value.Is("android");
			android.Children.Count().Is(1); // 変化
			android.Albums.Is();
			android.Children.First().Is(androidGame); // Index変化
			androidGame.Title.Value.Is("game");
			androidGame.Children.Is();
			androidGame.Albums.Is(androidGameFF7);

			// パス変更
			iphonePictureRiver.AlbumPath.Value = "/iphone2/picture";

			shelf.Title.Value.Is("root");
			shelf.Children.Count().Is(3);
			shelf.Children.First().Is(iphone);
			iphone.Title.Value.Is("iphone");
			iphone.Children.Count().Is(1); // 変化
			iphone.Albums.Count().Is(1);
			iphone.Albums.Is(iphoneOther);
			iphone.Children.First().Is(iphonePicture);
			iphonePicture.Title.Value.Is("picture");
			iphonePicture.Children.Is();
			iphonePicture.Albums.Is(iphonePictureMountain);
			shelf.Children.Skip(1).First().Is(android);
			android.Title.Value.Is("android");
			android.Children.Count().Is(1);
			android.Albums.Is();
			android.Children.First().Is(androidGame);
			androidGame.Title.Value.Is("game");
			androidGame.Children.Is();
			androidGame.Albums.Is(androidGameFF7);
			var iphone2 = shelf.Children.Skip(2).First(); // 追加
			iphone2.Title.Value.Is("iphone2");
			iphone2.Children.Count().Is(1);
			iphone2.Albums.Is();
			var iphone2Picture = iphone2.Children.First();
			iphone2Picture.Title.Value = "picture";
			iphone2Picture.Children.Is();
			iphone2Picture.Albums.Is(iphonePictureRiver);
		}
	}
}
