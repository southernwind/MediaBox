using System.Windows.Media;

using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// 画像ファイルViewModel
	/// </summary>
	internal class ImageFileViewModel : MediaFileViewModel<ImageFileModel> {

		/// <summary>
		/// フルサイズイメージ
		/// </summary>
		public ImageSource Image {
			get {
				return this.ConcreteModel.Image;
			}
		}

		public ImageFileViewModel(ImageFileModel mediaFile) : base(mediaFile) {
		}
	}
}
