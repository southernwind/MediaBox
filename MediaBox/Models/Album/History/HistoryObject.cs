
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.History;

namespace SandBeige.MediaBox.Models.Album.History {
	public record HistoryObject(IAlbumObject AlbumObject, string Title) : IHistoryObject;
}
