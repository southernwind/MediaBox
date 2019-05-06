using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SandBeige.MediaBox.Library.Creator {
	/// <summary>
	/// <see cref="ImageSource"/>クリエイター
	/// </summary>
	public static class ImageSourceCreator {

		/// <summary>
		/// 作成
		/// </summary>
		/// <param name="source">作成元データ(ファイルパス:<see cref="string"/>, バイナリデータ(<see cref="Stream"/>)</param>
		/// <param name="orientation">回転方向(ExifのOrientation)</param>
		/// <param name="width">作成する<see cref="ImageSource"/>の幅</param>
		/// <param name="height">作成する<see cref="ImageSource"/>の高さ</param>
		/// <param name="token">キャンセルトークン</param>
		/// <returns>作成されたImageSource</returns>
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

		/// <summary>
		/// 作成
		/// </summary>
		/// <param name="source">作成元データ(ファイルパス:<see cref="string"/>, バイナリデータ(<see cref="Stream"/>)</param>
		/// <param name="orientation">回転方向(ExifのOrientation)</param>
		/// <param name="width">作成する<see cref="ImageSource"/>の幅</param>
		/// <param name="height">作成する<see cref="ImageSource"/>の高さ</param>
		/// <param name="token">キャンセルトークン</param>
		/// <returns>作成されたImageSource</returns>
		public static ImageSource Create(
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
			return Create(stream, orientation, width, height, token);
		}

		/// <summary>
		/// 作成
		/// </summary>
		/// <param name="stream">作成元バイナリデータ(<see cref="Stream"/>)</param>
		/// <param name="orientation">回転方向(ExifのOrientation)</param>
		/// <param name="width">作成する<see cref="ImageSource"/>の幅</param>
		/// <param name="height">作成する<see cref="ImageSource"/>の高さ</param>
		/// <param name="token">キャンセルトークン</param>
		/// <returns>作成されたImageSource</returns>
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

			image.Rotation = GetRotation(orientation);

			token.ThrowIfCancellationRequested();
			image.EndInit();
			token.ThrowIfCancellationRequested();
			image.Freeze();

			// 反転させる必要がないならそのまま
			if (!NeedsFlipX(orientation)) {
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

		/// <summary>
		/// 回転方向の取得
		/// </summary>
		/// <param name="orientation">Orientation</param>
		/// <returns>回転方向</returns>
		public static Rotation GetRotation(int? orientation) {
			switch (orientation) {
				default:
					return Rotation.Rotate0;
				case 3:
				case 4:
					return Rotation.Rotate180;
				case 5:
				case 8:
					return Rotation.Rotate270;
				case 6:
				case 7:
					return Rotation.Rotate90;
			}
		}

		/// <summary>
		/// 反転が必要かどうか
		/// </summary>
		/// <param name="orientation">Orientation</param>
		/// <returns>反転が必要かどうか</returns>
		public static bool NeedsFlipX(int? orientation) {
			return new int?[] { 2, 4, 5, 7 }.Contains(orientation);
		}
	}
}
