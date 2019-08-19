using System;
using System.Linq;

using SandBeige.MediaBox.Models.Album.History.Creator;

namespace SandBeige.MediaBox.Models.Album.History {
	/// <summary>
	/// アルバム履歴管理
	/// </summary>
	internal class AlbumHistoryManager : ModelBase {
		/// <summary>
		/// アルバム履歴追加
		/// </summary>
		/// <param name="album">追加対象アルバム</param>
		public void Add(IAlbumModel album) {
			// TODO : 同一アルバム判定 雑な判定なので、いいように作り変える
			if (this.States.AlbumStates.AlbumHistory.FirstOrDefault()?.Title == album?.Title.Value) {
				return;
			}
			IAlbumCreator ac;
			switch (album) {
				case RegisteredAlbum ra:
					ac = new RegisterAlbumCreator(ra.Title.Value, ra.AlbumId.Value);
					break;
				case FolderAlbum fa:
					ac = new FolderAlbumCreator(fa.Title.Value, fa.DirectoryPath);
					break;
				case LookupDatabaseAlbum lda:
					var ldac = new LookupDatabaseAlbumCreator(lda.Title.Value);
					ldac.TagName = lda.TagName;
					ldac.Word = lda.Word;
					ldac.Address = lda.Address;
					ac = ldac;
					break;
				default:
					throw new ArgumentException(album?.ToString());
			}
			this.States.AlbumStates.AlbumHistory.Insert(0, ac);
			// 10件目以降は削除
			foreach (var h in this.States.AlbumStates.AlbumHistory.Skip(10).ToArray()) {
				this.States.AlbumStates.AlbumHistory.Remove(h);
			}
		}
	}
}
