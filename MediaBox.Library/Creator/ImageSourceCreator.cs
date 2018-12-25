using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SandBeige.MediaBox.Library.Creator {
	public static class ImageSourceCreator {

		public static async Task<ImageSource> CreateAsync(
			object source,
			int? orientation = null,
			double width = 0,
			double height = 0,
			CancellationToken token = default) {

			Stream stream;
			switch (source) {
				case string path:
					stream = new FileStream(path, FileMode.Open, FileAccess.Read);
					break;
				case Stream sr:
					stream = sr;
					break;
				default:
					throw new ArgumentException();
			}

			try {
				return await Task.Run(async () => {
					using (var ms = new MemoryStream()) {
						await stream.CopyToAsync(ms, 8920, token);
						stream.Dispose();
						ms.Position = 0;

						return Create(ms, orientation, width, height, token);
					}
				}, token);
			} catch (Exception ex) when (ex is OperationCanceledException | ex is ObjectDisposedException) {
				// キャンセル
				return null;
			}
		}

		public static ImageSource Create(
			object source,
			int? orientation = null,
			double width = 0,
			double height = 0) {
			Stream stream;
			switch (source) {
				case string path:
					stream = new FileStream(path, FileMode.Open, FileAccess.Read);
					break;
				case Stream sr:
					stream = sr;
					break;
				default:
					throw new ArgumentException();
			}
			return Create(stream, orientation, width, height);
		}

		private static ImageSource Create(
			Stream stream,
			int? orientation = null,
			double width = 0,
			double height = 0,
			CancellationToken token = default) {
			var image = new BitmapImage();
			image.BeginInit();

			token.ThrowIfCancellationRequested();

			image.StreamSource = stream;

			image.CacheOption = BitmapCacheOption.OnLoad;
			image.CreateOptions = BitmapCreateOptions.None;

			image.DecodePixelWidth = (int)width;
			image.DecodePixelHeight = (int)height;

			switch (orientation) {
				default:
					image.Rotation = Rotation.Rotate0;
					break;
				case 3:
				case 4:
					image.Rotation = Rotation.Rotate180;
					break;
				case 5:
				case 8:
					image.Rotation = Rotation.Rotate270;
					break;
				case 6:
				case 7:
					image.Rotation = Rotation.Rotate90;
					break;
			}

			token.ThrowIfCancellationRequested();
			image.EndInit();
			token.ThrowIfCancellationRequested();
			image.Freeze();

			// 反転させる必要がないならそのまま
			if (!new int?[] { 2, 4, 5, 7 }.Contains(orientation)) {
				token.ThrowIfCancellationRequested();
				return image;
			}

			// 反転
			token.ThrowIfCancellationRequested();
			var tb = new TransformedBitmap(image, new ScaleTransform(-1, 1, 0, 0));
			token.ThrowIfCancellationRequested();
			tb.Freeze();
			token.ThrowIfCancellationRequested();
			return tb;
		}
	}
}
