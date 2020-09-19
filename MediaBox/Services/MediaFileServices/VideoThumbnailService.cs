using System.Collections.Generic;
using System.IO;
using System.Linq;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Services.MediaFileServices;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Services.MediaFileServices {
	public class VideoThumbnailService : ServiceBase, IVideoThumbnailService {
		private readonly ISettings _settings;

		public VideoThumbnailService(ISettings settings) {
			this._settings = settings;
		}

		/// <summary>
		/// 等間隔サムネイル作成
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <param name="resolution">元動画解像度</param>
		/// <param name="duration">動画の長さ</param>
		/// <returns>サムネイルのベースファイル名</returns>
		public string Create(string filePath, ComparableSize resolution, double duration) {
			var num = this._settings.GeneralSettings.NumberOfVideoThumbnail.Value;

			var timeList = Enumerable.Range(1, num).Select((x, i) => (key: i, value: duration * x / (num + 1))).ToDictionary(x => x.key, x => x.value);
			return this.CreateCore(filePath, resolution, timeList);
		}

		/// <summary>
		/// サムネイル番号と時刻を指定してサムネイル作成
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <param name="resolution">元動画解像度</param>
		/// <param name="index">サムネイル番号</param>
		/// <param name="time">時間</param>
		/// <returns>サムネイルのベースファイル名</returns>
		public string Create(string filePath, ComparableSize resolution, int index, double time) {
			return this.CreateCore(filePath, resolution, new Dictionary<int, double> {
				{ index,time }
			});
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <param name="resolution">元動画解像度</param>
		/// <param name="timeList">取得するサムネイルの番号と時刻のリスト</param>
		/// <returns>サムネイルのベースファイル名</returns>
		private string CreateCore(string filePath, ComparableSize resolution, Dictionary<int, double> timeList) {
			var path = Thumbnail.GetThumbnailRelativeFilePath(filePath);
			var ffmpeg = new Library.Video.Ffmpeg(this._settings.PathSettings.FfmpegDirectoryPath.Value);

			foreach (var (index, time) in timeList.Select(x => (x.Key, x.Value))) {
				ffmpeg.CreateThumbnail(
					filePath,
					Path.Combine(this._settings.PathSettings.ThumbnailDirectoryPath.Value, $"{path}{(index == 0 ? "" : $"_{index}")}"),
					(int)resolution.Width,
					(int)resolution.Height,
					this._settings.GeneralSettings.ThumbnailWidth.Value,
					this._settings.GeneralSettings.ThumbnailHeight.Value,
					time);
			}
			return path;
		}
	}
}
