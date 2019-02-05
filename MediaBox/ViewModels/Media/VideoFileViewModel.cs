using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// 動画ファイルViewModel
	/// </summary>
	internal class VideoFileViewModel : MediaFileViewModel<VideoFileModel> {
		/// <summary>
		/// 回転
		/// </summary>
		public int? Rotation {
			get {
				return -this.ConcreteModel.Rotation;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mediaFile">モデルインスタンス</param>
		public VideoFileViewModel(VideoFileModel mediaFile) : base(mediaFile) {
		}
	}
}
