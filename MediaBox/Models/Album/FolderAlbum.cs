using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	internal class FolderAlbum : AlbumModel {
		/// <summary>
		/// サムネイル作成キュー
		/// subjectのOnNextで発火してitemsの中身をすべて登録する
		/// </summary>
		private (Subject<Unit> subject, IList<MediaFileModel> items) QueueOfCreateThumbnail {
			get;
		} = (new Subject<Unit>(), new List<MediaFileModel>());

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
							Parallel.ForEach(
								this.QueueOfCreateThumbnail.items.ToArray(),
								new ParallelOptions {
									CancellationToken = this.CancellationToken,
									MaxDegreeOfParallelism = Environment.ProcessorCount
								}, mediaFile => {
									if (this.CancellationToken.IsCancellationRequested) {
										return;
									}
									mediaFile.GetFileInfoIfNotLoaded();
									mediaFile.CreateThumbnailIfNotExists();
									this.QueueOfCreateThumbnail.items.Remove(mediaFile);
								});
						} catch (Exception ex) when (ex is OperationCanceledException) {
							this.Logging.Log("フォルダアルバムのメディア情報読み込みキャンセル", LogLevel.Debug, ex);
						}
					}
				});
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		protected override void LoadFileInDirectory(string directoryPath) {
			this.ThumbnailLocation = ThumbnailLocation.Memory;
			this.Items.AddRange(
				Directory
					.EnumerateFiles(directoryPath)
					.Where(x => x.IsTargetExtension())
					.Where(x => this.Items.All(m => m.FilePath != x))
					.Select(x => this.MediaFactory.Create(x, this.ThumbnailLocation))
					.ToList());
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
					this.Items.Add(this.MediaFactory.Create(e.FullPath, this.ThumbnailLocation));
					break;
				case WatcherChangeTypes.Deleted:
					this.Items.Remove(this.Items.Single(i => i.FilePath == e.FullPath));
					break;
			}
		}

		/// <summary>
		/// メディアファイル追加
		/// </summary>
		/// <param name="mediaFile"></param>
		protected override void OnAddedItem(MediaFileModel mediaFile) {
			this.QueueOfCreateThumbnail.items.Add(mediaFile);
			this.QueueOfCreateThumbnail.subject.OnNext(Unit.Default);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}
	}
}
