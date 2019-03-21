using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.IO;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// フォルダアルバム
	/// </summary>
	/// <remarks>
	/// </remarks>
	internal class FolderAlbum : AlbumModel {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FolderAlbum(string path) {
			this.Title.Value = path;
			this.MonitoringDirectories.Add(path);

			// メディアファイルの追加時の情報読み込み/サムネイル作成
			this.Items
				.ObserveAddChanged<IMediaFileModel>()
				.Subscribe(x => {
					var ta = new TaskAction(
						$"サムネイル・ファイル情報読み込み[{x.FileName}]",
						() => {
							x.GetFileInfoIfNotLoaded();
							x.CreateThumbnailIfNotExists();
						},
						Priority.LoadFolderAlbumFileInfo,
						this.CancellationToken);
					this.PriorityTaskQueue.AddTask(ta);
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		/// <param name="cancellationToken">キャンセルトークン</param>
		protected override void LoadFileInDirectory(string directoryPath, CancellationToken cancellationToken) {
			var newItems = DirectoryEx
				.EnumerateFiles(directoryPath)
				.Where(x => x.IsTargetExtension())
				.Where(x => this.Items.All(m => m.FilePath != x))
				.Select(x => this.MediaFactory.Create(x));
			foreach (var item in newItems) {
				if (cancellationToken.IsCancellationRequested) {
					return;
				}

				lock (this.Items.SyncRoot) {
					this.Items.Add(item);
				}
			}
		}

		/// <summary>
		/// ファイルシステムイベント
		/// </summary>
		/// <param name="e">作成・更新・改名・削除などのイベント情報</param>
		protected override void OnFileSystemEvent(FileSystemEventArgs e) {
			if (!e.FullPath.IsTargetExtension()) {
				return;
			}

			switch (e.ChangeType) {
				case WatcherChangeTypes.Created:
					lock (this.Items.SyncRoot) {
						this.Items.Add(this.MediaFactory.Create(e.FullPath));
					}
					break;
				case WatcherChangeTypes.Deleted:
					lock (this.Items.SyncRoot) {
						this.Items.Remove(this.Items.Single(i => i.FilePath == e.FullPath));
					}
					break;
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}
	}
}
