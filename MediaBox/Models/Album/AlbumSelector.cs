using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.History;
using SandBeige.MediaBox.Models.Album.History.Creator;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Notification;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.ViewModels;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバム選択
	/// </summary>
	/// <remarks>
	/// <see cref="AlbumContainer"/>に含まれているアルバムと、指定フォルダから生成した<see cref="FolderAlbum"/>のうちから
	/// 一つの<see cref="AlbumModel"/>を<see cref="CurrentAlbum"/>として選ぶ。
	/// <see cref="FolderAlbum"/>の場合はカレントでなくなった時点で<see cref="IDisposable.Dispose"/>される。
	/// </remarks>
	public abstract class AlbumSelector : ModelBase, IAlbumSelector {
		private readonly MediaBoxDbContext _rdb;
		private readonly DocumentDb _documentDb;
		private readonly MediaFactory _mediaFactory;
		private readonly NotificationManager _notificationManager;
		private readonly ISettings _settings;
		private readonly ILogging _logging;
		private readonly IGestureReceiver _gestureReceiver;
		private readonly MediaFileManager _mediaFileManager;
		private readonly ViewModelFactory _viewModelFactory;
		private readonly PriorityTaskQueue _priorityTaskQueue;
		private readonly AlbumViewerManager _albumViewerManager;
		private readonly GeoCodingManager _geoCodingManager;
		/// <summary>
		/// コンテナ
		/// </summary>
		private readonly AlbumContainer _albumContainer;

		/// <summary>
		/// フィルター
		/// </summary>
		public IFilterSetter FilterSetter {
			get;
		}

		/// <summary>
		/// ソート
		/// </summary>
		public ISortSetter SortSetter {
			get;
		}

		/// <summary>
		/// アルバムリスト
		/// </summary>
		public ReadOnlyReactiveCollection<RegisteredAlbum> AlbumList {
			get;
		}

		/// <summary>
		/// カレントアルバム
		/// </summary>
		public IReactiveProperty<IAlbumModel> CurrentAlbum {
			get;
		} = new ReactiveProperty<IAlbumModel>();

		/// <summary>
		/// ルートアルバムボックス
		/// </summary>
		public IReactiveProperty<AlbumBox> Shelf {
			get;
		} = new ReactivePropertySlim<AlbumBox>();

		/// <summary>
		/// Folder
		/// </summary>
		public IReactiveProperty<FolderObject> Folder {
			get;
		} = new ReactivePropertySlim<FolderObject>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="albumContainer">アルバムコンテナ</param>
		/// <param name="albumHistoryManager">アルバム履歴管理</param>
		/// <param name="filterSetter">フィルターマネージャー</param>
		/// <param name="sortSetter">ソートマネージャー</param>
		public AlbumSelector(
			AlbumContainer albumContainer,
			AlbumHistoryManager albumHistoryManager,
			FilterDescriptionManager filterSetter,
			SortDescriptionManager sortSetter,
			MediaBoxDbContext rdb,
			DocumentDb documentDb,
			ISettings settings,
			ILogging logging,
			IGestureReceiver gestureReceiver,
			MediaFactory mediaFactory,
			NotificationManager notificationManager,
			MediaFileManager mediaFileManager,
			ViewModelFactory viewModelFactory,
			PriorityTaskQueue priorityTaskQueue,
			AlbumViewerManager albumViewerManager,
			GeoCodingManager geoCodingManager) {
			this._rdb = rdb;
			this._mediaFactory = mediaFactory;
			this._gestureReceiver = gestureReceiver;
			this._documentDb = documentDb;
			this._notificationManager = notificationManager;
			this._albumContainer = albumContainer;
			this._settings = settings;
			this._logging = logging;
			this._mediaFileManager = mediaFileManager;
			this._viewModelFactory = viewModelFactory;
			this._priorityTaskQueue = priorityTaskQueue;
			this._albumViewerManager = albumViewerManager;
			this._geoCodingManager = geoCodingManager;
			this.FilterSetter = filterSetter;
			this.SortSetter = sortSetter;

			// アルバムIDリストからアルバムリストの生成
			this.AlbumList = this._albumContainer.AlbumList.ToReadOnlyReactiveCollection(x => {
				var ra = new RegisteredAlbum(this, this._settings, this._logging, this._gestureReceiver, this._rdb, this._mediaFactory, this._documentDb, this._notificationManager, this._mediaFileManager, this._albumContainer, this._viewModelFactory, this._priorityTaskQueue, this._albumViewerManager, this._geoCodingManager);
				ra.LoadFromDataBase(x);
				return ra;
			}).AddTo(this.CompositeDisposable);

			// 初期値
			this.Shelf.Value = new AlbumBox(this.AlbumList, this._rdb).AddTo(this.CompositeDisposable);

			IEnumerable<ValueCountPair<string>> func() {
				lock (this._rdb) {
					return this._documentDb
						.GetMediaFilesCollection()
						.Query()
						// TODO : 暫定対応
						.ToEnumerable()
						.GroupBy(x => x.DirectoryPath)
						.Select(x => new ValueCountPair<string>(x.Key, x.Count()))
						.ToList();
				}
			}

			this.Folder.Value = new FolderObject("", func());

			mediaFileManager.OnRegisteredMediaFiles
				.Merge(mediaFileManager.OnDeletedMediaFiles)
				.Throttle(TimeSpan.FromMilliseconds(100))
				.Synchronize()
				.ObserveOn(UIDispatcherScheduler.Default)
				.Subscribe(_ => {
					using (this.DisposeLock.DisposableEnterReadLock()) {
						if (this.DisposeState != DisposeState.NotDisposed) {
							return;
						}
						this.Folder.Value.Update(func());
					}
				});

			// カレントアルバム切り替え時、登録アルバム以外ならDisposeしておく
			this.CurrentAlbum
				.Pairwise()
				.Subscribe(x => {
					if (!(x.OldItem is RegisteredAlbum)) {
						x.OldItem?.Dispose();
					}
				}).AddTo(this.CompositeDisposable);

			this.OnDisposed
				.Subscribe(_ => {
					this.CurrentAlbum.Value?.Dispose();
				}).AddTo(this.CompositeDisposable);

			this.CurrentAlbum.Where(x => x != null).Subscribe(albumHistoryManager.Add).AddTo(this.CompositeDisposable);

			this.FilterSetter.OnFilteringConditionChanged
				.Merge(this.SortSetter.OnSortConditionChanged)
				.Subscribe(_ => {
					this.CurrentAlbum.Value?.LoadMediaFiles();
				});
		}

		/// <summary>
		/// 引数のアルバムをカレントする
		/// </summary>
		/// <param name="album"></param>
		public void SetAlbumToCurrent(IAlbumModel album) {
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// 引数のアルバムをカレントする
		/// </summary>
		/// <param name="albumCreator"></param>
		public void SetAlbumToCurrent(IAlbumCreator albumCreator) {
			this.CurrentAlbum.Value = albumCreator?.Create(this, this._settings, this._logging, this._gestureReceiver, this._rdb, this._mediaFactory, this._documentDb, this._notificationManager, this._mediaFileManager, this._albumContainer, this._viewModelFactory, this._priorityTaskQueue, this._albumViewerManager, this._geoCodingManager);
		}

		/// <summary>
		/// フォルダアルバムをカレントにする
		/// </summary>
		public void SetFolderAlbumToCurrent(string path) {
			if (path == null) {
				return;
			}
			var album = new FolderAlbum(path, this, this._settings, this._logging, this._gestureReceiver, this._rdb, this._mediaFactory, this._documentDb, this._notificationManager, this._viewModelFactory, this._priorityTaskQueue, this._mediaFileManager, this._albumViewerManager, this._geoCodingManager);
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// データベース検索アルバムをカレントにする
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public void SetDatabaseAlbumToCurrent(string tagName) {
			var album = new LookupDatabaseAlbum(this, this._settings, this._logging, this._gestureReceiver, this._rdb, this._mediaFactory, this._documentDb, this._notificationManager, this._viewModelFactory, this._priorityTaskQueue, this._mediaFileManager, this._albumViewerManager, this._geoCodingManager);
			album.Title.Value = $"タグ : {tagName}";
			album.TagName = tagName;
			album.LoadFromDataBase();
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// ワード検索アルバムをカレントにする
		/// </summary>
		/// <param name="word">検索ワード</param>
		public void SetWordSearchAlbumToCurrent(string word) {
			var album = new LookupDatabaseAlbum(this, this._settings, this._logging, this._gestureReceiver, this._rdb, this._mediaFactory, this._documentDb, this._notificationManager, this._viewModelFactory, this._priorityTaskQueue, this._mediaFileManager, this._albumViewerManager, this._geoCodingManager);
			album.Title.Value = $"検索ワード : {word}";
			album.Word = word;
			album.LoadFromDataBase();
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// 場所検索アルバムをカレントにする
		/// </summary>
		/// <param name="address">場所情報</param>
		public void SetPositionSearchAlbumToCurrent(Address address) {
			var album = new LookupDatabaseAlbum(this, this._settings, this._logging, this._gestureReceiver, this._rdb, this._mediaFactory, this._documentDb, this._notificationManager, this._viewModelFactory, this._priorityTaskQueue, this._mediaFileManager, this._albumViewerManager, this._geoCodingManager);
			album.Title.Value = $"場所 : {address.Name}";
			album.Address = address;
			album.LoadFromDataBase();
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="album">削除対象アルバム</param>
		public void DeleteAlbum(IAlbumModel album) {
			if (!(album is RegisteredAlbum ra)) {
				throw new ArgumentException();
			}
			lock (this._rdb) {
				this._rdb.Remove(this._rdb.Albums.Single(x => x.AlbumId == ra.AlbumId.Value));
				this._rdb.SaveChanges();
			}
			this._albumContainer.RemoveAlbum(ra.AlbumId.Value);
		}

		/// <summary>
		/// 名称設定
		/// </summary>
		/// <param name="name">一意になる名称 フィルターとソート順の保存、復元に使用する。</param>
		protected void SetName(string name) {
			this.FilterSetter.Name.Value = name;
			this.SortSetter.Name.Value = name;
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.CurrentAlbum.Value?.Title.Value}>";
		}
	}

	/// <summary>
	/// メインウィンドウ用アルバムセレクター
	/// </summary>
	public class MainAlbumSelector : AlbumSelector {
		public MainAlbumSelector(
			AlbumContainer albumContainer,
			AlbumHistoryManager albumHistoryManager,
			FilterDescriptionManager filterSetter,
			SortDescriptionManager sortSetter,
			MediaBoxDbContext rdb,
			DocumentDb documentDb,
			ISettings settings,
			ILogging logging,
			IGestureReceiver gestureReceiver,
			MediaFactory mediaFactory,
			NotificationManager notificationManager,
			MediaFileManager mediaFileManager,
			ViewModelFactory viewModelFactory,
			PriorityTaskQueue priorityTaskQueue,
			AlbumViewerManager albumViewerManager,
			GeoCodingManager geoCodingManager)
			: base(albumContainer, albumHistoryManager, filterSetter, sortSetter, rdb, documentDb, settings, logging, gestureReceiver, mediaFactory, notificationManager, mediaFileManager, viewModelFactory, priorityTaskQueue, albumViewerManager, geoCodingManager) {
			this.SetName("main");
		}
	}

	/// <summary>
	/// 編集ウィンドウ用アルバムセレクター
	/// </summary>
	public class EditorAlbumSelector : AlbumSelector {
		public EditorAlbumSelector(
			AlbumContainer albumContainer,
			AlbumHistoryManager albumHistoryManager,
			FilterDescriptionManager filterSetter,
			SortDescriptionManager sortSetter,
			MediaBoxDbContext rdb,
			DocumentDb documentDb,
			ISettings settings,
			ILogging logging,
			IGestureReceiver gestureReceiver,
			MediaFactory mediaFactory,
			NotificationManager notificationManager,
			MediaFileManager mediaFileManager,
			ViewModelFactory viewModelFactory,
			PriorityTaskQueue priorityTaskQueue,
			AlbumViewerManager albumViewerManager,
			GeoCodingManager geoCodingManager)
			: base(albumContainer, albumHistoryManager, filterSetter, sortSetter, rdb, documentDb, settings, logging, gestureReceiver, mediaFactory, notificationManager, mediaFileManager, viewModelFactory, priorityTaskQueue, albumViewerManager, geoCodingManager) {
			this.SetName("editor");
		}
	}
}
