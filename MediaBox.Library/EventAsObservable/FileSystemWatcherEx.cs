using System;
using System.IO;
using System.Reactive.Linq;

namespace SandBeige.MediaBox.Library.EventAsObservable {
	/// <summary>
	/// <see cref="FileSystemWatcher"/>の拡張クラス
	/// </summary>
	public static class FileSystemWatcherEx {
		/// <summary>
		/// 作成
		/// </summary>
		/// <param name="watcher">イベント発生源<see cref="FileSystemWatcher"/></param>
		/// <returns>イベントIO</returns>
		public static IObservable<FileSystemEventArgs> CreatedAsObservable(this FileSystemWatcher watcher) {
			return Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
				h => (sender, e) => h(e),
				h => watcher.Created += h,
				h => watcher.Created -= h);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="watcher">イベント発生源<see cref="FileSystemWatcher"/></param>
		/// <returns>イベントIO</returns>
		public static IObservable<FileSystemEventArgs> DeletedAsObservable(this FileSystemWatcher watcher) {
			return Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
				h => (sender, e) => h(e),
				h => watcher.Deleted += h,
				h => watcher.Deleted -= h);
		}

		/// <summary>
		/// リネーム
		/// </summary>
		/// <param name="watcher">イベント発生源<see cref="FileSystemWatcher"/></param>
		/// <returns>イベントIO</returns>
		public static IObservable<RenamedEventArgs> RenamedAsObservable(this FileSystemWatcher watcher) {
			return Observable.FromEvent<RenamedEventHandler, RenamedEventArgs>(
				h => (sender, e) => h(e),
				h => watcher.Renamed += h,
				h => watcher.Renamed -= h);
		}

		/// <summary>
		/// 変更
		/// </summary>
		/// <param name="watcher">イベント発生源<see cref="FileSystemWatcher"/></param>
		/// <returns>イベントIO</returns>
		public static IObservable<FileSystemEventArgs> ChangedAsObservable(this FileSystemWatcher watcher) {
			return Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
				h => (sender, e) => h(e),
				h => watcher.Changed += h,
				h => watcher.Changed -= h);
		}

		/// <summary>
		/// Dispose
		/// </summary>
		/// <param name="watcher">イベント発生源<see cref="FileSystemWatcher"/></param>
		/// <returns>イベントIO</returns>
		public static IObservable<EventArgs> DisposedAsObservable(this FileSystemWatcher watcher) {
			return Observable.FromEvent<EventHandler, EventArgs>(
				h => (sender, e) => h(e),
				h => watcher.Disposed += h,
				h => watcher.Disposed -= h);
		}
	}
}
