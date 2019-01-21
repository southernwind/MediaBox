using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

using SandBeige.MediaBox.Library.Creator;

namespace SandBeige.MediaBox.Models.Media {
	internal class ImageFileModel : MediaFileModel {
		private ImageSource _image;
		private CancellationTokenSource _loadImageCancelToken;
		public ImageFileModel(string filePath) : base(filePath) {
		}

		/// <summary>
		/// 画像読み込み
		/// </summary>
		public override async Task LoadImageAsync() {
			if (this._loadImageCancelToken != null) {
				return;
			}
			this._loadImageCancelToken = new CancellationTokenSource();
			this.Image =
				await ImageSourceCreator.CreateAsync(
					this.FilePath,
					this.Orientation,
					token: this._loadImageCancelToken.Token);
			this._loadImageCancelToken = null;

		}

		/// <summary>
		/// 読み込んだ画像破棄
		/// </summary>
		public override void UnloadImage() {
			this._loadImageCancelToken?.Cancel();
			this.Image = null;
		}

		/// <summary>
		/// 表示用画像
		/// </summary>
		public ImageSource Image {
			get {
				return this._image;
			}
			set {
				if (this._image == value) {
					return;
				}
				this._image = value;
				this.RaisePropertyChanged();
			}
		}
	}
}
