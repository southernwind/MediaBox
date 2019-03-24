using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading;

using Livet;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.IO;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// フォルダアルバム
	/// </summary>
	/// <remarks>
	/// </remarks>
	internal class FolderAlbum : AlbumModel {
		public string DirectoryPath {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FolderAlbum(string path) : base(new ObservableSynchronizedCollection<IMediaFileModel>()) {
			this.Title.Value = path;
			this.DirectoryPath = path;
			this.Load();

			var mfm = Get.Instance<MediaFileManager>();
			mfm
				.OnRegisteredMediaFile
				.Where(x => x.FilePath.StartsWith($@"{this.DirectoryPath}\"))
				.Subscribe(x => {
					lock (this.Items.SyncRoot) {
						this.Items.Add(x);
					}
				});

			mfm
				.OnFileSystemEvent
				.Where(x => x.ChangeType == WatcherChangeTypes.Deleted)
				.Subscribe(x => {
					lock (this.Items.SyncRoot) {
						// TODO : 作成後すぐに削除を行うと登録前に削除通知がくるかもしれないので考慮が必要
						this.Items.RemoveRange(this.Items.Where(i => i.FilePath.StartsWith($@"{x.FullPath}\") || i.FilePath == x.FullPath));
					}
				});
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		/// <param name="cancellationToken">キャンセルトークン</param>
		protected void LoadFileInDirectory(string directoryPath, CancellationToken cancellationToken) {
			var newItems = DirectoryEx
				.EnumerateFiles(directoryPath)
				.Where(x => x.IsTargetExtension())
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
		protected void OnFileSystemEvent(FileSystemEventArgs e) {
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

		/// <summary>
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected override Expression<Func<MediaFile, bool>> WherePredicate() {
			return mediaFile => mediaFile.DirectoryPath.StartsWith(this.DirectoryPath);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}

	}
}
