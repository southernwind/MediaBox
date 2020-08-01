namespace SandBeige.MediaBox.Composition.Interfaces.Models.Media {
	public interface IMediaFactory {
		/// <summary>
		/// <see cref="IMediaFileModel"/>の取得
		/// </summary>
		/// <param name="key">ファイルパス</param>
		/// <returns>生成された<see cref="IMediaFileModel"/></returns>
		public IMediaFileModel Create(string key);
	}
}
