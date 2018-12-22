﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Library.EventAsObservable;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバムクラス
	/// </summary>
	internal abstract class Album : MediaFileCollection {
		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// ファイル更新監視
		/// </summary>
		protected ReadOnlyReactiveCollection<FileSystemWatcher> FileSystemWatchers {
			get;
		}

		/// <summary>
		/// ファイル更新監視ディレクトリ
		/// </summary>
		public ReactiveCollection<string> MonitoringDirectories {
			get;
		} = new ReactiveCollection<string>();

		/// <summary>
		/// マップ
		/// </summary>
		public ReactivePropertySlim<MapModel> Map {
			get;
		} = new ReactivePropertySlim<MapModel>(Get.Instance<MapModel>());

		/// <summary>
		/// カレントのメディアファイル(単一)
		/// </summary>
		public ReactivePropertySlim<MediaFile> CurrentMediaFile {
			get;
		} = new ReactivePropertySlim<MediaFile>();

		/// <summary>
		/// カレントのメディアファイル(複数)
		/// </summary>
		public ReactiveCollection<MediaFile> CurrentMediaFiles {
			get;
		} = new ReactiveCollection<MediaFile>();

		/// <summary>
		/// カレントのメディアファイルのプロパティ
		/// </summary>
		public ReactivePropertySlim<MediaFileProperties> MediaFileProperties {
			get;
		} = new ReactivePropertySlim<MediaFileProperties>(Get.Instance<MediaFileProperties>());

		/// <summary>
		/// 表示モード
		/// </summary>
		public ReadOnlyReactivePropertySlim<DisplayMode> DisplayMode {
			get;
		}

		protected Album() {
			this.DisplayMode = this.Settings.GeneralSettings.DisplayMode.ToReadOnlyReactivePropertySlim();
			this.Items
				.ToCollectionChanged()
				.ObserveOn(Dispatcher.CurrentDispatcher, DispatcherPriority.Background)
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(async x => {
					if (x.Action == NotifyCollectionChangedAction.Add) {
						await this.OnAddedItemAsync(x.Value);
					}
				}).AddTo(this.CompositeDisposable);

			// アイテム→マップアイテム片方向同期
			this.Items.SynchronizeTo(this.Map.Value.Items).AddTo(this.CompositeDisposable);

			// カレントアイテム→プロパティ片方向同期
			this.CurrentMediaFiles.SynchronizeTo(this.MediaFileProperties.Value.Items).AddTo(this.CompositeDisposable);

			// カレントアイテムフルイメージロード
			this.CurrentMediaFile
				.ToOldAndNewValue()
				.CombineLatest(
					this.DisplayMode,
					(currentItem, displayMode) => (currentItem, displayMode))
				.Subscribe(async x => {
						x.currentItem.OldValue?.UnloadImage();
					if (x.displayMode == Composition.Enum.DisplayMode.Detail) {
						await x.currentItem.NewValue.LoadImageAsync();
					}
				});

			// カレントアイテム→マップカレントアイテム同期
			this.CurrentMediaFile
				.Subscribe(x => {
					this.Map.Value.CurrentMediaFile.Value = x;
				});

			// Exifロード
			this.CurrentMediaFile
				.Where(x => x != null)
				.Subscribe(async x => {
					await x.LoadExifIfNotLoadedAsync();
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
				}).AddTo(this.CompositeDisposable);
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

		public void ChangeDisplayMode(DisplayMode displayMode) {
			this.Settings.GeneralSettings.DisplayMode.Value = displayMode;
		}
	}
}
