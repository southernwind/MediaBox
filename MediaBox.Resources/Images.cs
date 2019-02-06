using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SandBeige.MediaBox.Resources {
	/// <summary>
	/// 画像リソースクラス
	/// </summary>
	public static class Images {
		private static readonly Assembly _assembly;
		private static ImageSource _noImage;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		static Images() {
			_assembly = Assembly.GetExecutingAssembly();
		}

		/// <summary>
		/// ファイルが見当たらないときの代替画像
		/// </summary>
		public static ImageSource NoImage {
			get {
				return _noImage ?? (_noImage = CreateImageSource(_assembly.GetManifestResourceStream("SandBeige.MediaBox.Resources.Files.NoImage.jpg")));
			}
		}

		/// <summary>
		/// <see cref="ImageSource"/>作成メソッド
		/// </summary>
		/// <param name="stream">画像ファイルストリーム</param>
		/// <returns>作成された<see cref="ImageSource"/></returns>
		private static ImageSource CreateImageSource(Stream stream) {
			var image = new BitmapImage();
			image.BeginInit();
			image.StreamSource = stream;
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.CreateOptions = BitmapCreateOptions.None;
			image.EndInit();
			image.Freeze();
			return image;
		}
	}
}
