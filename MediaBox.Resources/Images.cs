using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SandBeige.MediaBox.Resources {
	public static class Images {
		private static readonly Assembly _assembly;
		private static ImageSource _noImage;

		static Images() {
			_assembly = Assembly.GetExecutingAssembly();
		}

		public static ImageSource NoImage {
			get {
				return _noImage ?? (_noImage = CreateImageSource(_assembly.GetManifestResourceStream("SandBeige.MediaBox.Resources.Files.NoImage.jpg")));
			}
		}

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
