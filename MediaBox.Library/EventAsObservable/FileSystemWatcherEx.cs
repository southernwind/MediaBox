using System;
using System.IO;
using System.Reactive.Linq;

namespace SandBeige.MediaBox.Library.EventAsObservable {
	/// <summary>
	/// <see cref="FileSystemWatcher"/>の拡張クラス
	/// </summary>
	public static class FileSystemWatcherEx {
		public static IObservable<FileSystemEventArgs> CreatedAsObservable(this FileSystemWatcher watcher) {
			return Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
				h => (sender, e) => h(e),
				h => watcher.Created += h,
				h => watcher.Created -= h);
		}

		public static IObservable<FileSystemEventArgs> DeletedAsObservable(this FileSystemWatcher watcher) {
			return Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
				h => (sender, e) => h(e),
				h => watcher.Deleted += h,
				h => watcher.Deleted -= h);
		}

		public static IObservable<RenamedEventArgs> RenamedAsObservable(this FileSystemWatcher watcher) {
			return Observable.FromEvent<RenamedEventHandler, RenamedEventArgs>(
				h => (sender, e) => h(e),
				h => watcher.Renamed += h,
				h => watcher.Renamed -= h);
		}

		public static IObservable<FileSystemEventArgs> ChangedAsObservable(this FileSystemWatcher watcher) {
			return Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
				h => (sender, e) => h(e),
				h => watcher.Changed += h,
				h => watcher.Changed -= h);
		}
		public static IObservable<EventArgs> DisposedAsObservable(this FileSystemWatcher watcher) {
			return Observable.FromEvent<EventHandler, EventArgs>(
				h => (sender, e) => h(e),
				h => watcher.Disposed += h,
				h => watcher.Disposed -= h);
		}
	}
}
