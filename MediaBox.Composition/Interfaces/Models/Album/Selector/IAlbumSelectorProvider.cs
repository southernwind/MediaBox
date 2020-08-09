namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector {
	public interface IAlbumSelectorProvider {
		/// <summary>
		/// アルバムセレクター作成
		/// </summary>
		/// <param name="name">セレクター名</param>
		/// <returns>アルバムセレクター</returns>
		public IAlbumSelector Create(string name);
	}
}
