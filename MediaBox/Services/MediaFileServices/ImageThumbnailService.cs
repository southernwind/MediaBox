using System.IO;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Services.MediaFileServices;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Image;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Services.MediaFileServices {
	public class ImageThumbnailService : ServiceBase, IImageThumbnailService {
		private readonly ISettings _settings;

		public ImageThumbnailService(ISettings settings) {
			this._settings = settings;
		}

		public string Create(string filePath) {
			var path = Thumbnail.GetThumbnailRelativeFilePath(filePath);
			using var fs = File.OpenRead(filePath);
#if LOAD_LOG
			this._logging.Log($"[Thumbnail Create]{this.FileName}");
#endif
			var image = ThumbnailCreator.Create(fs, this._settings.GeneralSettings.ThumbnailWidth.Value, this._settings.GeneralSettings.ThumbnailHeight.Value);
			File.WriteAllBytes(Path.Combine(this._settings.PathSettings.ThumbnailDirectoryPath.Value, path), image);

			return path;
		}
	}
}
