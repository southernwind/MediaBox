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
				return this.ConcreteModel.Rotation;
			}
		}

		/// <summary>
		/// パフォーマンス改善のため、Bindingエラーを起こさないよう、ImageFileViewModelに合わせて作成
		/// サムネイルファイルパスを渡しておく
		/// </summary>
		public object Image {
			get {
				return this.ConcreteModel.ThumbnailFilePath;
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
