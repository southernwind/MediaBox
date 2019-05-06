using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.History;

namespace SandBeige.MediaBox.Tests.Models.Album.History {
	internal class AlbumHistoryManagerTest : ModelTestClassBase {

		[Test]
		public void 追加() {
			this.States.AlbumStates.AlbumHistory.Count.Is(0);

			var ahm = new AlbumHistoryManager();
			var ra = new RegisteredAlbum();
			ra.Create();
			ra.Title.Value = "登録アルバム";
			ra.ReflectToDataBase();
			var fa = new FolderAlbum(@"C:\test\");
			var lda = new LookupDatabaseAlbum();
			lda.TagName = "tag";
			lda.Title.Value = "tag:tag";
			ahm.Add(ra);
			ahm.Add(fa);
			ahm.Add(lda);

			this.States.AlbumStates.AlbumHistory.Count.Is(3);
			var ah1 = this.States.AlbumStates.AlbumHistory[0].Create();
			var ah2 = this.States.AlbumStates.AlbumHistory[1].Create();
			var ah3 = this.States.AlbumStates.AlbumHistory[2].Create();
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

			var ahm = new AlbumHistoryManager();
			foreach (var _ in Enumerable.Range(1, 11)) {
				var ra = new RegisteredAlbum();
				ra.Create();
				ra.Title.Value = "登録アルバム";
				ra.ReflectToDataBase();
				ahm.Add(ra);
			}

			this.States.AlbumStates.AlbumHistory.Count.Is(10);
			var ah1 = this.States.AlbumStates.AlbumHistory[0].Create();
			var ah2 = this.States.AlbumStates.AlbumHistory[1].Create();
			var ah3 = this.States.AlbumStates.AlbumHistory[2].Create();
			var ah4 = this.States.AlbumStates.AlbumHistory[3].Create();
			var ah5 = this.States.AlbumStates.AlbumHistory[4].Create();
			var ah6 = this.States.AlbumStates.AlbumHistory[5].Create();
			var ah7 = this.States.AlbumStates.AlbumHistory[6].Create();
			var ah8 = this.States.AlbumStates.AlbumHistory[7].Create();
			var ah9 = this.States.AlbumStates.AlbumHistory[8].Create();
			var ah10 = this.States.AlbumStates.AlbumHistory[9].Create();
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
