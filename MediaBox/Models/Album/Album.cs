using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.EventAsObservable;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバムクラス
	/// </summary>
	abstract class Album : ModelBase {
		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 件数
		/// </summary>
		public IReactiveProperty<int> Count {
			get;
		} = new ReactivePropertySlim<int>();

		/// <summary>
		/// メディアファイルリスト
		/// </summary>
		public ReactiveCollection<MediaFile> Items {
			get;
		} = new ReactiveCollection<MediaFile>();

		/// <summary>
		/// 登録処理キュー
		/// </summary>
		protected ReactiveCollection<MediaFile> Queue {
			get;
		} = new ReactiveCollection<MediaFile>();

		/// <summary>
		/// ファイル更新監視
		/// </summary>
		protected ReadOnlyReactiveCollection<FileSystemWatcher> FileSystemWatchers {
			get;
			set;
		}

		/// <summary>
		/// ファイル更新監視ディレクトリ
		/// </summary>
		public ReactiveCollection<string> MonitoringDirectories {
			get;
		} = new ReactiveCollection<string>();

		public Album() {
			// キューに入ったメディアを処理しながらメディアファイルリストに移していく
			this.Queue
				.ToCollectionChanged()
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => {
					if (x.Action == NotifyCollectionChangedAction.Add) {
						this.AddItem(x.Value);
						this.Queue.RemoveOnScheduler(x.Value);
					}
				});

			this.Items
				.ToCollectionChanged()
				.Subscribe(x => {
					this.Count.Value = this.Items.Count;
				});

			// ファイル更新監視
			this.FileSystemWatchers = this
				.MonitoringDirectories
				.ToReadOnlyReactiveCollection(md => {
					if (!Directory.Exists(md)) {
						this.Logging.Log($"監視フォルダが見つかりません。{md}", LogLevel.Warning);
						return null;
					}
					// 初期読み込み
					this.LoadFileInDirectory(md);
					var fsw = new FileSystemWatcher(md) {
						IncludeSubdirectories = true,
						EnableRaisingEvents = true
					};
					var disposable = Observable.Merge(
						fsw.CreatedAsObservable(),
						fsw.RenamedAsObservable(),
						fsw.ChangedAsObservable(),
						fsw.DeletedAsObservable()
						).Subscribe(x => {
							if (!x.FullPath.IsTargetExtension()) {
								return;
							}
							if (x.ChangeType == WatcherChangeTypes.Created) {
								this.Queue.AddOnScheduler(Get.Instance<MediaFile>(x.FullPath));
							}
						});

					fsw.DisposedAsObservable().Subscribe(_ => disposable.Dispose());

					return fsw;
				});
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">フォルダパス</param>
		protected abstract void LoadFileInDirectory(string directoryPath);

		/// <summary>
		/// メディアファイル追加
		/// </summary>
		/// <param name="mediaFile">メディアファイル</param>
		protected abstract void AddItem(MediaFile mediaFile);
	}
}
