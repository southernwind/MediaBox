﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

using Livet;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {

	/// <summary>
	/// アルバムクラス
	/// </summary>
	/// <remarks>
	/// 複数の<see cref="IMediaFileModel"/>を保持、管理するクラス。
	/// <see cref="MediaFileCollection.Items"/>に持っている
	/// </remarks>
	internal abstract class AlbumModel : MediaFileCollection, IAlbumModel {
		private readonly CancellationTokenSource _cancellationTokenSource;

		private readonly FilterDescriptionManager FilterDescriptionManager = Get.Instance<FilterDescriptionManager>();

		/// <summary>
		/// キャンセルトークン Dispose時にキャンセルされる。
		/// </summary>
		protected CancellationToken CancellationToken {
			get;
		}

		private readonly ObservableSynchronizedCollection<PriorityWith<IMediaFileModel>> _loadingImages = new ObservableSynchronizedCollection<PriorityWith<IMediaFileModel>>();
		private readonly TaskAction _taskAction;
		protected readonly PriorityTaskQueue PriorityTaskQueue;

		/// <summary>
		/// フィルタリング前件数
		/// </summary>
		public IReactiveProperty<int> BeforeFilteringCount {
			get;
		} = new ReactivePropertySlim<int>();

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// マップ
		/// </summary>
		public IReactiveProperty<MapModel> Map {
			get;
		}

		/// <summary>
		/// カレントのメディアファイル(単一)
		/// </summary>
		public IReactiveProperty<IMediaFileModel> CurrentMediaFile {
			get;
		} = new ReactivePropertySlim<IMediaFileModel>();

		/// <summary>
		/// カレントのメディアファイル(複数)
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileModel>> CurrentMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>(Array.Empty<IMediaFileModel>());

		/// <summary>
		/// カレントのメディアファイルの情報
		/// </summary>
		public IReactiveProperty<MediaFileInformation> MediaFileInformation {
			get;
		} = new ReactivePropertySlim<MediaFileInformation>(Get.Instance<MediaFileInformation>());

		/// <summary>
		/// 表示モード
		/// </summary>
		public IReactiveProperty<DisplayMode> DisplayMode {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected AlbumModel(ObservableSynchronizedCollection<IMediaFileModel> items) : base(items) {
			this._cancellationTokenSource = new CancellationTokenSource();
			this._cancellationTokenSource.AddTo(this.CompositeDisposable);
			this.CancellationToken = this._cancellationTokenSource.Token;
			this.PriorityTaskQueue = Get.Instance<PriorityTaskQueue>();
			// フルイメージロード用タスク
			this._taskAction = new TaskAction(
				$"フルサイズイメージ読み込み[{this._loadingImages.Count}]",
				() => {
					while (true) {
						var m = this._loadingImages.Where(x => !x.Completed).OrderBy(p => p.Priority).FirstOrDefault();
						if (m?.Object is ImageFileModel ifm) {
							ifm.LoadImageIfNotLoaded();
							m.Completed = true;
						} else {
							break;
						}
					}
				},
				Priority.LoadFullImage,
				this.CancellationToken
			);

			this.DisplayMode =
				this.Settings
					.GeneralSettings
					.DisplayMode
					.ToReactivePropertyAsSynchronized(x => x.Value)
					.AddTo(this.CompositeDisposable);

			// アイテム→マップアイテム片方向同期
			this.Map = new ReactivePropertySlim<MapModel>(Get.Instance<MapModel>(this.Items));

			this.Map.Value.OnSelect.Select(x => x.ToArray()).Subscribe(x => {
				this.CurrentMediaFiles.Value =
					x.All(this.CurrentMediaFiles.Value.Contains) ?
						this.CurrentMediaFiles.Value.Except(x) :
						this.CurrentMediaFiles.Value.Union(x.Where(m => !this.CurrentMediaFiles.Value.Contains(m))).ToList();
			});

			// カレントアイテム→プロパティ,マップカレントアイテム片方向同期
			this.CurrentMediaFiles.Select(x => x.ToArray()).Subscribe(x => {
				this.Map.Value.CurrentMediaFiles.Value = x;
				this.MediaFileInformation.Value.Files.Value = x;
			});

			// カレントアイテムの先頭を取得
			this.CurrentMediaFiles
				.Subscribe(_ => {
					this.CurrentMediaFile.Value = this.CurrentMediaFiles.Value.FirstOrDefault();
				}).AddTo(this.CompositeDisposable);

			// カレントアイテム→マップカレントアイテム同期
			this.CurrentMediaFile
				.Subscribe(x => {
					this.Map.Value.CurrentMediaFile.Value = x;
				}).AddTo(this.CompositeDisposable);

			this.FilterDescriptionManager
				.OnFilteringConditionChanged
				.ObserveOn(TaskPoolScheduler.Default)
				.Synchronize()
				.Subscribe(_ => {
					// TODO : キャンセルの仕組みが必要か
					this.Load();
				});
		}

		/// <summary>
		/// フィルタリング前件数更新
		/// </summary>
		protected void UpdateBeforeFilteringCount() {
			lock (this.DataBase) {
				this.BeforeFilteringCount.Value = this.DataBase.MediaFiles.Count(this.WherePredicate());
			}
		}

		/// <summary>
		/// アルバム情報読み込み
		/// </summary>
		public void Load() {
			IEnumerable<IMediaFileModel> items;
			lock (this.DataBase) {
				this.UpdateBeforeFilteringCount();
				items = this
					.FilterDescriptionManager
					.SetFilterConditions(
						this.DataBase
							.MediaFiles
							.Where(this.WherePredicate())
					)
				.Include(mf => mf.MediaFileTags)
				.ThenInclude(mft => mft.Tag)
				.Include(mf => mf.ImageFile)
				.Include(mf => mf.VideoFile)
				.AsEnumerable()
				.Select(x => {
					var m = this.MediaFactory.Create(x.FilePath);
					m.LoadFromDataBase(x);
					m.UpdateFileInfo();
					return m;
				}).ToList();
			}

			lock (this.Items.SyncRoot) {
				this.Items.Clear();
				lock (this.DataBase) {
					this.Items.AddRange(items);
				}
			}
		}

		/// <summary>
		/// 表示モードの変更を行う。
		/// </summary>
		/// <param name="displayMode">変更後表示モード</param>
		public void ChangeDisplayMode(DisplayMode displayMode) {
			this.Settings.GeneralSettings.DisplayMode.Value = displayMode;
		}

		/// <summary>
		/// 事前読み込み
		/// </summary>
		/// <remarks>
		/// 事前読み込みしたい画像リストを受け取って受け取った順番どおりに事前読み込みを行う
		/// Remove
		/// </remarks>
		/// <param name="models">事前読み込みが必要なメディアリスト</param>
		public void Prefetch(IEnumerable<IMediaFileModel> models) {
			// 　↓要件
			// ・前回の読み込みの途中で、かつ今回も読み込みリストに入っている場合だった場合、そのまま読み込み継続させる
			// ・読み込みリストから消えた場合はフルイメージをアンロードする
			// ・読み込みは渡されたコレクションの順番の通りに行う

			// いなくなった分は削除
			var removeList = this._loadingImages.Where(li => !models.Contains(li.Object)).ToArray();
			foreach (var item in removeList) {
				this._loadingImages.Remove(item);
			}
			// 追加された分は追加
			foreach (var item in models.Except(this._loadingImages.Select(li => li.Object)).Select(li => new PriorityWith<IMediaFileModel>(li, 0))) {
				this._loadingImages.Add(item);
			}
			// 優先度を追加された順に変更
			foreach (var (model, index) in models.Select((x, i) => (x, i))) {
				var image = this._loadingImages.SingleOrDefault(x => x.Object == model);
				// 別スレッドで削除している可能性があるのでnull判定をしておく
				if (image == null) {
					continue;
				}
				image.Priority = index;
			}
			// いらなくなったイメージはアンロードしておく
			foreach (var m in removeList.Select(x => x)) {
				if (m.Object is ImageFileModel ifm) {
					ifm.UnloadImage();
				}
			}

			if (this._loadingImages.Count == 0) {
				return;
			}

			if (this.PriorityTaskQueue.Contains(this._taskAction) && this._taskAction.TaskState == TaskState.WorkInProgress) {
				return;
			}
			this._taskAction.OnTaskCompleted.FirstAsync();
			this._taskAction.TaskState = TaskState.Waiting;
			this.PriorityTaskQueue.AddTask(this._taskAction);
		}

		/// <summary>
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected abstract Expression<Func<MediaFile, bool>> WherePredicate();

		/// <summary>
		/// Dispose
		/// </summary>
		/// <remarks>
		/// 実行中の非同期処理をキャンセルしてからDisposeする。
		/// </remarks>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing) {
			if (this.Disposed) {
				return;
			}
			this._cancellationTokenSource.Cancel();
			base.Dispose(disposing);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}

		public class PriorityWith<T> {
			public T Object {
				get;
			}

			public int Priority {
				get;
				set;
			}

			public bool Completed {
				get;
				set;
			}

			public PriorityWith(T obj, int priority) {
				this.Object = obj;
				this.Priority = priority;
			}
		}
	}
}
