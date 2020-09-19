using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces.Services.MediaFileServices {
	public interface IVideoThumbnailService : IServiceBase {
		/// <summary>
		/// 等間隔サムネイル作成
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <param name="resolution">元動画解像度</param>
		/// <param name="duration">動画の長さ</param>
		/// <returns>サムネイルのベースファイル名</returns>
		string Create(string filePath, ComparableSize resolution, double duration);

		/// <summary>
		/// サムネイル番号と時刻を指定してサムネイル作成
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <param name="resolution">元動画解像度</param>
		/// <param name="index">サムネイル番号</param>
		/// <param name="time">時間</param>
		/// <returns>サムネイルのベースファイル名</returns>
		string Create(string filePath, ComparableSize resolution, int index, double time);
	}
}