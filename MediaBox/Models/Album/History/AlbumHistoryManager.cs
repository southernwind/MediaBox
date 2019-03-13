using System;
using System.Linq;

using SandBeige.MediaBox.Utilities;

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
			IAlbumCreator ac;
			switch (album) {
				case RegisteredAlbum ra:
					ac = Get.Instance<RegisterAlbumCreator>(ra.Title.Value, ra.AlbumId.Value);
					break;
				case FolderAlbum fa:
					ac = Get.Instance<FolderAlbumCreator>(fa.Title.Value, fa.MonitoringDirectories.First());
					break;
				case LookupDatabaseAlbum lda:
					var ldac = Get.Instance<LookupDatabaseAlbumCreator>(lda.Title.Value);
					ldac.TagName = lda.TagName;
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
