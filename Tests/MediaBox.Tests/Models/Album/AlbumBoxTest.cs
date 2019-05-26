
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumBoxTest : ModelTestClassBase {
		[Test]
		public void 階層構造() {
			var ac = Get.Instance<AlbumContainer>();
			var selector = new AlbumSelector("main");
			var shelf = selector.Shelf.Value;
			var iphoneOther = new RegisteredAlbum(selector);
			iphoneOther.Create();
			iphoneOther.AlbumPath.Value = "/iphone";
			iphoneOther.Title.Value = "other";
			iphoneOther.ReflectToDataBase();
			var iphonePictureSea = new RegisteredAlbum(selector);
			iphonePictureSea.Create();
			iphonePictureSea.AlbumPath.Value = "/iphone/picture";
			iphonePictureSea.Title.Value = "sea";
			iphonePictureSea.ReflectToDataBase();
			var iphoneGameMspoke = new RegisteredAlbum(selector);
			iphoneGameMspoke.Create();
			iphoneGameMspoke.AlbumPath.Value = "/iphone/game";
			iphoneGameMspoke.Title.Value = "mspoke";
			iphoneGameMspoke.ReflectToDataBase();
			var iphonePictureRiver = new RegisteredAlbum(selector);
			iphonePictureRiver.Create();
			iphonePictureRiver.AlbumPath.Value = "/iphone/picture";
			iphonePictureRiver.Title.Value = "river";
			iphonePictureRiver.ReflectToDataBase();
			var androidPictureSea = new RegisteredAlbum(selector);
			androidPictureSea.Create();
			androidPictureSea.AlbumPath.Value = "/android/picture";
			androidPictureSea.Title.Value = "sea";
			androidPictureSea.ReflectToDataBase();
			var iphonePictureMountain = new RegisteredAlbum(selector);
			iphonePictureMountain.Create();
			iphonePictureMountain.AlbumPath.Value = "/iphone/picture";
			iphonePictureMountain.Title.Value = "mountain";
			iphonePictureMountain.ReflectToDataBase();
			var androidGameFf7 = new RegisteredAlbum(selector);
			androidGameFf7.Create();
			androidGameFf7.AlbumPath.Value = "/android/game";
			androidGameFf7.Title.Value = "FF7";
			androidGameFf7.ReflectToDataBase();

			ac.AddAlbum(iphoneOther.AlbumId.Value);
			iphoneOther = selector.AlbumList.Single(x => x.AlbumId.Value == iphoneOther.AlbumId.Value);
			ac.AddAlbum(iphonePictureSea.AlbumId.Value);
			iphonePictureSea = selector.AlbumList.Single(x => x.AlbumId.Value == iphonePictureSea.AlbumId.Value);
			ac.AddAlbum(iphoneGameMspoke.AlbumId.Value);
			iphoneGameMspoke = selector.AlbumList.Single(x => x.AlbumId.Value == iphoneGameMspoke.AlbumId.Value);
			ac.AddAlbum(iphonePictureRiver.AlbumId.Value);
			iphonePictureRiver = selector.AlbumList.Single(x => x.AlbumId.Value == iphonePictureRiver.AlbumId.Value);
			ac.AddAlbum(androidPictureSea.AlbumId.Value);
			androidPictureSea = selector.AlbumList.Single(x => x.AlbumId.Value == androidPictureSea.AlbumId.Value);

			// 3つアルバム追加した時点での確認
			selector.AlbumList.Is(iphoneOther, iphonePictureSea, iphoneGameMspoke, iphonePictureRiver, androidPictureSea);
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
			ac.AddAlbum(iphonePictureMountain.AlbumId.Value);
			iphonePictureMountain = selector.AlbumList.Single(x => x.AlbumId.Value == iphonePictureMountain.AlbumId.Value);
			ac.AddAlbum(androidGameFf7.AlbumId.Value);
			androidGameFf7 = selector.AlbumList.Single(x => x.AlbumId.Value == androidGameFf7.AlbumId.Value);

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
			androidGame.Albums.Is(androidGameFf7);

			// 削除
			ac.RemoveAlbum(iphonePictureSea.AlbumId.Value);
			ac.RemoveAlbum(iphoneGameMspoke.AlbumId.Value);
			ac.RemoveAlbum(androidPictureSea.AlbumId.Value);
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
			androidGame.Albums.Is(androidGameFf7);

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
			androidGame.Albums.Is(androidGameFf7);
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
