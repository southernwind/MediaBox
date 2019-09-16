using System.IO;
using System.Reflection;
using System.Text;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Models.Settings {
	public class ViewerSettings : SettingsBase, IViewerSettings {

		/// <summary>
		/// MediaFile表示Xaml
		/// </summary>

		public SettingsItem<string> MediaFileViewerControlXaml {
			get;
		} = new SettingsItem<string>(LoadTextResource("SandBeige.MediaBox.Models.Settings.DefaultMediaFileControlXaml.xaml"));

		private static string LoadTextResource(string path) {
			var assembly = Assembly.GetExecutingAssembly();
			using var stream = assembly.GetManifestResourceStream(path);
			using var sr = new StreamReader(stream, Encoding.UTF8);
			return sr.ReadToEnd();
		}
	}
}
