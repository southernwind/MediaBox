
using System;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.History;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;

namespace SandBeige.MediaBox.Models.Album.History {
	public class HistoryObject : IHistoryObject {
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

		[Obsolete("for serialize")]
		public HistoryObject() {
			this.AlbumObject = null!;
			this.Title = null!;
		}

		public HistoryObject(IAlbumObject albumObject, string title) {
			this.AlbumObject = albumObject;
			this.Title = title;
		}
	}
}
