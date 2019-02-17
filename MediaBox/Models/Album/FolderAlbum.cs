using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.IO;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// フォルダアルバム
	/// </summary>
	/// <remarks>
	/// </remarks>
	internal class FolderAlbum : AlbumModel {
		/// <summary>
		/// サムネイル作成キュー
		/// subjectのOnNextで発火してitemsの中身をすべて登録する
		/// </summary>
		private (Subject<Unit> subject, ConcurrentQueue<IMediaFileModel> items) QueueOfCreateThumbnail {
			get;
		} = (new Subject<Unit>(), new ConcurrentQueue<IMediaFileModel>());

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FolderAlbum(string path) {
			this.Title.Value = path;
			this.MonitoringDirectories.Add(path);

			this.QueueOfCreateThumbnail
				.subject
				.Throttle(TimeSpan.FromMilliseconds(10))
				.ObserveOnBackground(this.Settings.ForTestSettings.RunOnBackground.Value)
				.Subscribe(_ => {
					lock (this.QueueOfCreateThumbnail.items) {
						try {
							Parallel.For(
								0,
								this.QueueOfCreateThumbnail.items.Count,
								new ParallelOptions {
									CancellationToken = this.CancellationToken,
									MaxDegreeOfParallelism = Environment.ProcessorCount
								}, __ => {
									this.QueueOfCreateThumbnail.items.TryDequeue(out var mediaFile);
									if (this.CancellationToken.IsCancellationRequested) {
										return;
									}
									mediaFile.GetFileInfoIfNotLoaded();
									mediaFile.CreateThumbnailIfNotExists();
								});
						} catch (Exception ex) when (ex is OperationCanceledException) {
							this.Logging.Log("フォルダアルバムのメディア情報読み込みキャンセル", LogLevel.Debug, ex);
						}
					}
				}).AddTo(this.CompositeDisposable);

			// メディアファイルの追加時の情報読み込み/サムネイル作成
			this.Items
				.ToCollectionChanged<IMediaFileModel>()
				.Subscribe(x => {
					switch (x.Action) {
						case NotifyCollectionChangedAction.Add:
							this.QueueOfCreateThumbnail.items.Enqueue(x.Value);
							this.QueueOfCreateThumbnail.subject.OnNext(Unit.Default);
							break;
					}
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		/// <param name="cancellationToken">キャンセルトークン</param>
		protected override void LoadFileInDirectory(string directoryPath, CancellationToken cancellationToken) {
			this.ThumbnailLocation = Composition.Enum.ThumbnailLocation.Memory;

			var newItems = DirectoryEx
				.EnumerateFiles(directoryPath)
				.Where(x => x.IsTargetExtension())
				.Where(x => this.Items.All(m => m.FilePath != x))
				.Select(x => this.MediaFactory.Create(x, this.ThumbnailLocation));
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
						this.Items.Add(this.MediaFactory.Create(e.FullPath, this.ThumbnailLocation));
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
