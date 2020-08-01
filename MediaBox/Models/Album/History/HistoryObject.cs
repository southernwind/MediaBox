
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Models.Album.History {
	public class HistoryObject {
		/// <summary>
		/// アルバムオブジェクト
		/// </summary>
		public IAlbumObject AlbumObject {
			get;
			set;
		}

		/// <summary>
		/// タイトル
		/// </summary>
		public string Title {
			get;
			set;
		}


	}
}
