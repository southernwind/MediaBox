using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
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
	internal abstract class Album : ModelBase {
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
			this.Items
				.ToCollectionChanged()
				.ObserveOn(Dispatcher.CurrentDispatcher,DispatcherPriority.Background)
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(async x => {
					this.Count.Value = this.Items.Count;
					if (x.Action == NotifyCollectionChangedAction.Add) {
						await this.OnAddedItemAsync(x.Value);
					}
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
								this.Items.AddOnScheduler(Get.Instance<MediaFile>(x.FullPath));
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
		/// リストにメディアファイルが追加されたときに呼ばれる。
		/// </summary>
		/// <param name="mediaFile">追加されたメディアファイル</param>
		protected abstract Task OnAddedItemAsync(MediaFile mediaFile);
	}
}
