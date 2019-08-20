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
			ahm.Add(ra);
			ahm.Add(fa);
			ahm.Add(lda);

			this.States.AlbumStates.AlbumHistory.Count.Is(3);
			using var ah1 = this.States.AlbumStates.AlbumHistory[0].Create(selector);
			using var ah2 = this.States.AlbumStates.AlbumHistory[1].Create(selector);
			using var ah3 = this.States.AlbumStates.AlbumHistory[2].Create(selector);
			(ah1 is LookupDatabaseAlbum).IsTrue();
			(ah1 as LookupDatabaseAlbum).TagName.Is("tag");
			(ah1 as LookupDatabaseAlbum).Title.Value.Is("tag:tag");
			(ah2 is FolderAlbum).IsTrue();
			(ah2 as FolderAlbum).DirectoryPath.Is(@"C:\test\");
			(ah3 is RegisteredAlbum).IsTrue();
			(ah3 as RegisteredAlbum).AlbumId.Value.Is(1);
			(ah3 as RegisteredAlbum).Title.Value.Is("登録アルバム");
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
			using var ah1 = this.States.AlbumStates.AlbumHistory[0].Create(selector);
			using var ah2 = this.States.AlbumStates.AlbumHistory[1].Create(selector);
			using var ah3 = this.States.AlbumStates.AlbumHistory[2].Create(selector);
			using var ah4 = this.States.AlbumStates.AlbumHistory[3].Create(selector);
			using var ah5 = this.States.AlbumStates.AlbumHistory[4].Create(selector);
			using var ah6 = this.States.AlbumStates.AlbumHistory[5].Create(selector);
			using var ah7 = this.States.AlbumStates.AlbumHistory[6].Create(selector);
			using var ah8 = this.States.AlbumStates.AlbumHistory[7].Create(selector);
			using var ah9 = this.States.AlbumStates.AlbumHistory[8].Create(selector);
			using var ah10 = this.States.AlbumStates.AlbumHistory[9].Create(selector);
			(ah1 as RegisteredAlbum).AlbumId.Value.Is(11);
			(ah2 as RegisteredAlbum).AlbumId.Value.Is(10);
			(ah3 as RegisteredAlbum).AlbumId.Value.Is(9);
			(ah4 as RegisteredAlbum).AlbumId.Value.Is(8);
			(ah5 as RegisteredAlbum).AlbumId.Value.Is(7);
			(ah6 as RegisteredAlbum).AlbumId.Value.Is(6);
			(ah7 as RegisteredAlbum).AlbumId.Value.Is(5);
			(ah8 as RegisteredAlbum).AlbumId.Value.Is(4);
			(ah9 as RegisteredAlbum).AlbumId.Value.Is(3);
			(ah10 as RegisteredAlbum).AlbumId.Value.Is(2);
		}
	}
}
