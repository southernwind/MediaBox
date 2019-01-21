using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// 動画ファイルViewModel
	/// </summary>
	internal class VideoFileViewModel : MediaFileViewModel<VideoFileModel> {
		public VideoFileViewModel(VideoFileModel mediaFile) : base(mediaFile) {
		}
	}
}
