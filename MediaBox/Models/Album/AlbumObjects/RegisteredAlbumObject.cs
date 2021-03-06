using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;

namespace SandBeige.MediaBox.Models.Album.AlbumObjects {
	public class RegisteredAlbumObject : IEditableAlbumObject, IBoxingableAlbumObject {
		/// <summary>
		/// アルバムID
		/// </summary>
		public int AlbumId {
			get;
			set;
		}
	}
}
