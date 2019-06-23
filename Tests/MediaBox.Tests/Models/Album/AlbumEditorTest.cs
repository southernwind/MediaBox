using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumEditorTest : ModelTestClassBase {
		[Test]
		public void アルバム作成() {
			using var albumSelector = new AlbumSelector("main");
			albumSelector.AlbumList.Count.Is(0);

			using (var editor = Get.Instance<AlbumEditor>()) {
				editor.CreateAlbum();
				editor.Title.Value = "album";
				editor.Save();
			}

			albumSelector.AlbumList.Count.Is(1);
			albumSelector.AlbumList.First().Title.Value.Is("album");
		}

		[Test]
		public void アルバムタイトル() {
			using var albumSelector = new AlbumSelector("main");
			using (var editor = Get.Instance<AlbumEditor>()) {
				editor.CreateAlbum();
				editor.Title.Value = "sad";
				editor.Save();
			}

			var album = albumSelector.AlbumList.First();
			using (var editor = Get.Instance<AlbumEditor>()) {
				editor.EditAlbum(album);
				editor.Title.Value.Is("");
				editor.Load();
				editor.Title.Value.Is("sad");
			}
		}

		[Test]
		public void アルバムパス() {
			using var albumSelector = new AlbumSelector("main");
			using (var editor = Get.Instance<AlbumEditor>()) {
				editor.CreateAlbum();
				editor.AlbumPath.Value = "/iphone/picture";
				editor.Save();
			}

			var album = albumSelector.AlbumList.First();
			using (var editor = Get.Instance<AlbumEditor>()) {
				editor.EditAlbum(album);
				editor.AlbumPath.Value.Is("");
				editor.Load();
				editor.AlbumPath.Value.Is("/iphone/picture");
			}
		}

		[Test]
		public void 監視ディレクトリ() {
			using var albumSelector = new AlbumSelector("main");
			using (var editor = Get.Instance<AlbumEditor>()) {
				editor.CreateAlbum();
				editor.AddDirectory(@"C:\test\");
				editor.AddDirectory(@"C:\image\");
				editor.AddDirectory(@"D:\picture\");
				editor.Save();
			}

			var album = albumSelector.AlbumList.First();
			using (var editor = Get.Instance<AlbumEditor>()) {
				editor.EditAlbum(album);
				editor.MonitoringDirectories.Is();
				editor.Load();
				editor.MonitoringDirectories.OrderBy(x => x).Is(@"C:\image\", @"C:\test\", @"D:\picture\");

				editor.RemoveDirectory(@"C:\test\");
				editor.RemoveDirectory(@"D:\picture\");
				editor.Save();
			}
			using (var editor = Get.Instance<AlbumEditor>()) {
				editor.EditAlbum(album);
				editor.MonitoringDirectories.Is();
				editor.Load();
				editor.MonitoringDirectories.Is(@"C:\image\");
			}
		}

		[Test]
		public async Task メディアファイル() {
			// 事前データ準備
			var (_, media1) = this.Register(this.TestFiles.Image1Jpg);
			var (_, media2) = this.Register(this.TestFiles.Image2Jpg);
			var (_, media3) = this.Register(this.TestFiles.Image3Jpg);
			var (_, media4) = this.Register(this.TestFiles.Image4Png);

			using var albumSelector = new AlbumSelector("main");
			using (var editor = Get.Instance<AlbumEditor>()) {
				editor.CreateAlbum();
				editor.AddFiles(new[] { media1, media2, media3, media4 });
				editor.Save();
			}

			await this.WaitTaskCompleted(3000);
			var album = albumSelector.AlbumList.First();
			await this.WaitTaskCompleted(3000);
			using (var editor = new AlbumEditor()) {
				editor.EditAlbum(album);
				editor.Items.Is();
				editor.Load();
				editor.Items.Is(media1, media2, media3, media4);

				editor.RemoveFiles(new[] { media1, media3 });
				editor.Save();
			}

			await this.WaitTaskCompleted(3000);
			using (var editor = new AlbumEditor()) {
				editor.EditAlbum(album);
				editor.Items.Is();
				editor.Load();
				editor.Items.Is(media2, media4);
			}
		}

		[Test]
		public void アルバム複数() {
			using var editor = new AlbumEditor();
			using var albumSelector = new AlbumSelector("main");
			// はじめは1件もない
			albumSelector.AlbumList.Count.Is(0);
			albumSelector.CurrentAlbum.Value.IsNull();

			// 作成して保存する(1件目)
			editor.CreateAlbum();
			editor.Save();
			albumSelector.AlbumList.Count.Is(1);
			albumSelector.AlbumList.Select(x => x.AlbumId.Value).Is(1);

			// 作成して保存する(2件目)
			editor.CreateAlbum();
			editor.Save();
			albumSelector.AlbumList.Count.Is(2);
			albumSelector.AlbumList.Select(x => x.AlbumId.Value).Is(1, 2);

			// 2つ目のアルバムを取得しておく
			var album2 = albumSelector.AlbumList.Single(x => x.AlbumId.Value == 2);

			// 編集
			editor.Title.Value = "title";
			editor.AlbumPath.Value = "/iphone/picture";
			editor.Save();

			// 一度保存したアルバムは使い回され、アルバムコンテナには追加されない
			albumSelector.AlbumList.Count.Is(2);
		}
	}
}
