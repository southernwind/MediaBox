using System.Collections.Generic;
using System.Windows.Media;

using SandBeige.MediaBox.Library.Exif;
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

		/// <summary>
		/// Exif情報のタイトル・値ペアリスト
		/// </summary>
		public IEnumerable<TitleValuePair> Exif {
			get {
				return this.ConcreteModel.Exif?.ToTitleValuePair();
			}
		}
		public ImageFileViewModel(ImageFileModel mediaFile) : base(mediaFile) {
		}
	}
}
