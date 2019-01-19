using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Reactive.Bindings;

using SandBeige.MediaBox.Models.Album;

using static SandBeige.MediaBox.Models.Album.AlbumModel;

namespace SandBeige.MediaBox.Tests.TestUtility {
	internal static class AlbumAwaiter {
		internal static async Task ProcessingMonitoringDirectory(this AlbumModel album) {
			var fsws = (ReadOnlyReactiveCollection<Fsw>)
				typeof(AlbumModel)
					.GetField("_fileSystemWatchers", BindingFlags.NonPublic | BindingFlags.Instance)
					.GetValue(album);
			await Task.Delay(100);
			foreach (var fsw in fsws.Where(x => x?.Task != null)) {
				await fsw.Task;
			}
		}
	}
}