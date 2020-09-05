using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Models.Album.AlbumObjects {
	/// <summary>
	/// データベース検索アルバム
	/// </summary>
	public record FolderAlbumObject(string FolderPath) : IAlbumObject;
}
