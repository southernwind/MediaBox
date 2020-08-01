using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
namespace SandBeige.MediaBox.Models.Album.AlbumObjects {
	public class RegisteredAlbumObject : IAlbumObject {
		/// <summary>
		/// アルバムID
		/// </summary>
		public int AlbumId {
			get;
			set;
		}
	}
}
