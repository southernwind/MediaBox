using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Reactive.Bindings;

using SandBeige.MediaBox.Models.Album;

namespace SandBeige.MediaBox.Tests.TestUtility {
	internal static class AlbumAwaiter {
		internal static async Task ProcessingMonitoringDirectory(this Album album) {
			var fsws = (ReadOnlyReactiveCollection<(Task<FileSystemWatcher>, CancellationTokenSource)>)
				typeof(Album)
					.GetField("_fileSystemWatchers", BindingFlags.NonPublic | BindingFlags.Instance)
					.GetValue(album);
			await Task.Delay(100);
			foreach (var (fsw, _) in fsws) {
				await fsw;
			}
		}
	}
}