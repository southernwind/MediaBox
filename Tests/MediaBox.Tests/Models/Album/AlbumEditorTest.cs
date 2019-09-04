using System.Linq;
using System.Reactive.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumEditorTest : ModelTestClassBase {
		[Test]
		public void アルバム作成() {
			using var albumSelector = new AlbumSelector("main");
			albumSelector.AlbumList.Count.Is(0);

			using (var editor = new AlbumEditor()) {
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
			using (var editor = new AlbumEditor()) {
				editor.CreateAlbum();
				editor.Title.Value = "sad";
				editor.Save();
			}

			var album = albumSelector.AlbumList.First();
			using (var editor = new AlbumEditor()) {
				editor.EditAlbum(album);
				editor.Title.Value.Is("");
				editor.Load();
				editor.Title.Value.Is("sad");
			}
		}

		[Test]
		public void アルバムボックスID() {
			lock (this.DataBase) {
				this.DataBase.AlbumBoxes.Add(
					new DataBase.Tables.AlbumBox() {
						AlbumBoxId = 2,
						Name = "いるか",
						ParentAlbumBoxId = null
					}
				);
			}
			this.DataBase.SaveChanges();
			using var albumSelector = new AlbumSelector("main");
			using (var editor = new AlbumEditor()) {
				editor.CreateAlbum();
				editor.AlbumBoxId.Value = 2;
				editor.Save();
			}

			var album = albumSelector.AlbumList.First();
			using (var editor = new AlbumEditor()) {
				editor.EditAlbum(album);
				editor.AlbumBoxId.Value.IsNull();
				editor.Load();
				editor.AlbumBoxId.Value.Is(2);
			}
		}

		[Test]
		public void 監視ディレクトリ() {
			using var albumSelector = new AlbumSelector("main");
			using (var editor = new AlbumEditor()) {
				editor.CreateAlbum();
				editor.AddDirectory(@"C:\test\");
				editor.AddDirectory(@"C:\image\");
				editor.AddDirectory(@"D:\picture\");
				editor.Save();
			}

			var album = albumSelector.AlbumList.First();
			using (var editor = new AlbumEditor()) {
				editor.EditAlbum(album);
				editor.MonitoringDirectories.Is();
				editor.Load();
				editor.MonitoringDirectories.OrderBy(x => x).Is(@"C:\image\", @"C:\test\", @"D:\picture\");

				editor.RemoveDirectory(@"C:\test\");
				editor.RemoveDirectory(@"D:\picture\");
				editor.Save();
			}
			using (var editor = new AlbumEditor()) {
				editor.EditAlbum(album);
				editor.MonitoringDirectories.Is();
				editor.Load();
				editor.MonitoringDirectories.Is(@"C:\image\");
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
			editor.Save();

			// 一度保存したアルバムは使い回され、アルバムコンテナには追加されない
			albumSelector.AlbumList.Count.Is(2);
		}
	}
}
