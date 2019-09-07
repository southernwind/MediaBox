using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.History;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album.History {
	internal class AlbumHistoryManagerTest : ModelTestClassBase {

		[Test]
		public void 追加() {
			this.States.AlbumStates.AlbumHistory.Count.Is(0);

			using var ahm = Get.Instance<AlbumHistoryManager>();
			using var selector = new AlbumSelector("main");
			using var ra = new RegisteredAlbum(selector);
			ra.Create();
			ra.Title.Value = "登録アルバム";
			ra.ReflectToDataBase();
			using var fa = new FolderAlbum(@"C:\test\", selector);
			using var lda = new LookupDatabaseAlbum(selector);
			lda.TagName = "tag";
			lda.Title.Value = "tag:tag";
			using var lda2 = new LookupDatabaseAlbum(selector);
			lda2.TagName = "tag";
			lda2.Title.Value = "tag:tag";
			ahm.Add(ra);
			ahm.Add(fa);
			ahm.Add(lda);
			// lda2は同一アルバムなので追加されない
			ahm.Add(lda2);

			this.States.AlbumStates.AlbumHistory.Count.Is(3);
			using var ah1 = this.States.AlbumStates.AlbumHistory[0].Create(selector).IsInstanceOf<LookupDatabaseAlbum>();
			using var ah2 = this.States.AlbumStates.AlbumHistory[1].Create(selector).IsInstanceOf<FolderAlbum>();
			using var ah3 = this.States.AlbumStates.AlbumHistory[2].Create(selector).IsInstanceOf<RegisteredAlbum>();
			ah1.TagName.Is("tag");
			ah1.Title.Value.Is("tag:tag");
			ah2.DirectoryPath.Is(@"C:\test\");
			ah3.AlbumId.Value.Is(1);
			ah3.Title.Value.Is("登録アルバム");
		}

		[Test]
		public void 履歴保存上限() {
			this.States.AlbumStates.AlbumHistory.Count.Is(0);
			using var selector = new AlbumSelector("main");

			using var ahm = Get.Instance<AlbumHistoryManager>();
			foreach (var index in Enumerable.Range(1, 11)) {
				using var ra = new RegisteredAlbum(selector);
				ra.Create();
				ra.Title.Value = $"登録アルバム{index}";
				ra.ReflectToDataBase();
				ahm.Add(ra);
			}

			this.States.AlbumStates.AlbumHistory.Count.Is(10);
			using var ah1 = this.States.AlbumStates.AlbumHistory[0].Create(selector).IsInstanceOf<RegisteredAlbum>();
			using var ah2 = this.States.AlbumStates.AlbumHistory[1].Create(selector).IsInstanceOf<RegisteredAlbum>();
			using var ah3 = this.States.AlbumStates.AlbumHistory[2].Create(selector).IsInstanceOf<RegisteredAlbum>();
			using var ah4 = this.States.AlbumStates.AlbumHistory[3].Create(selector).IsInstanceOf<RegisteredAlbum>();
			using var ah5 = this.States.AlbumStates.AlbumHistory[4].Create(selector).IsInstanceOf<RegisteredAlbum>();
			using var ah6 = this.States.AlbumStates.AlbumHistory[5].Create(selector).IsInstanceOf<RegisteredAlbum>();
			using var ah7 = this.States.AlbumStates.AlbumHistory[6].Create(selector).IsInstanceOf<RegisteredAlbum>();
			using var ah8 = this.States.AlbumStates.AlbumHistory[7].Create(selector).IsInstanceOf<RegisteredAlbum>();
			using var ah9 = this.States.AlbumStates.AlbumHistory[8].Create(selector).IsInstanceOf<RegisteredAlbum>();
			using var ah10 = this.States.AlbumStates.AlbumHistory[9].Create(selector).IsInstanceOf<RegisteredAlbum>();
			ah1.AlbumId.Value.Is(11);
			ah2.AlbumId.Value.Is(10);
			ah3.AlbumId.Value.Is(9);
			ah4.AlbumId.Value.Is(8);
			ah5.AlbumId.Value.Is(7);
			ah6.AlbumId.Value.Is(6);
			ah7.AlbumId.Value.Is(5);
			ah8.AlbumId.Value.Is(4);
			ah9.AlbumId.Value.Is(3);
			ah10.AlbumId.Value.Is(2);
		}
	}
}
