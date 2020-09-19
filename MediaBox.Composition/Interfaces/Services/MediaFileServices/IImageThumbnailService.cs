namespace SandBeige.MediaBox.Composition.Interfaces.Services.MediaFileServices {
	public interface IImageThumbnailService : IServiceBase {
		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <returns>サムネイルファイル名</returns>
		string Create(string filePath);
	}
}