using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumCreatorTest : TestClassBase {
		[Test]
		public async Task Test() {
			var media1 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
			var media2 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image2.jpg"));
			var media3 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image3.jpg"));
			var media4 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image4.jpg"));
			using (var creator = Get.Instance<AlbumCreator>())
			using (var albumSelector = Get.Instance<AlbumSelector>()) {
				// はじめは1件もない
				albumSelector.AlbumList.Count.Is(0);
				albumSelector.CurrentAlbum.Value.IsNull();

				// 作成して保存する(1件目)
				creator.CreateAlbum();
				creator.Save();
				await Task.Delay(10);
				albumSelector.AlbumList.Count.Is(1);
				albumSelector.AlbumList.Select(x => x.AlbumId.Value).Is(1);

				// 作成して保存する(2件目)
				creator.CreateAlbum();
				creator.Save();
				await Task.Delay(10);
				albumSelector.AlbumList.Count.Is(2);
				albumSelector.AlbumList.Select(x => x.AlbumId.Value).Is(1, 2);

				// 2つ目のアルバムを取得しておく
				var album2 = albumSelector.AlbumList.Single(x => x.AlbumId.Value == 2);

				// 編集
				creator.Title.Value = "title";
				creator.AlbumPath.Value = "/iphone/picture";
				creator.MonitoringDirectories.AddRange(new[] {
					TestDirectories["1"],
					TestDirectories["2"]
				});
				creator.AddFiles(new[] { media1, media2, media3 });

				// Saveする前はまだ反映していない
				album2.Title.Value.Is("");
				album2.AlbumPath.Value.Is("");
				album2.MonitoringDirectories.Is();
				album2.Items.Is();

				// Saveすると反映される
				creator.Save();
				album2.Title.Value.Is("title");
				album2.AlbumPath.Value.Is("/iphone/picture");
				album2.MonitoringDirectories.Is(TestDirectories["1"], TestDirectories["2"]);
				album2.Items.OrderBy(x => x.FileName).Is(media1, media2, media3);

				// 一度保存したアルバムは使い回され、アルバムコンテナには追加されない
				albumSelector.AlbumList.Count.Is(2);
			}

			using (var creator = Get.Instance<AlbumCreator>())
			using (var albumSelector = Get.Instance<AlbumSelector>()) {
				var album2 = albumSelector.AlbumList.Single(x => x.AlbumId.Value == 2);
				// 編集開始
				creator.EditAlbum(album2);
				// 読み込み前は空
				creator.Title.Value.Is("");
				creator.AlbumPath.Value.Is("");
				creator.MonitoringDirectories.Is();
				creator.Items.Is();

				// 読み込む
				creator.Load();
				creator.Title.Value.Is("title");
				creator.AlbumPath.Value.Is("/iphone/picture");
				creator.MonitoringDirectories.Is(TestDirectories["1"], TestDirectories["2"]);
				creator.Items.OrderBy(x => x.FileName).Is(media1, media2, media3);

				// 編集する
				creator.Title.Value = "タイトル";
				creator.AlbumPath.Value = "/携帯/写真";
				creator.MonitoringDirectories.Add(TestDirectories["3"]);
				creator.MonitoringDirectories.Remove(TestDirectories["1"]);
				creator.AddFiles(new[] { media4 });
				creator.RemoveFiles(new[] { media1, media2 });

				// 保存前は未反映
				album2.Title.Value.Is("title");
				album2.AlbumPath.Value.Is("/iphone/picture");
				album2.MonitoringDirectories.Is(TestDirectories["1"], TestDirectories["2"]);
				album2.Items.OrderBy(x => x.FileName).Is(media1, media2, media3);

				// 保存されると反映される
				creator.Save();
				album2.Title.Value.Is("タイトル");
				album2.AlbumPath.Value.Is("/携帯/写真");
				album2.MonitoringDirectories.Is(TestDirectories["2"], TestDirectories["3"]);
				album2.Items.OrderBy(x => x.FileName).Is(media3, media4);
			}
		}
	}
}
