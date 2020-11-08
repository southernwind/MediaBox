namespace SandBeige.MediaBox.Composition.Interfaces.Services.AlbumServices {
	public interface IAlbumEditorService {
		void RemoveFiles(int targetAlbumId, long[] mediaFileIds);
		void AddFiles(int targetAlbumId, long[] mediaFileIds);
	}
}