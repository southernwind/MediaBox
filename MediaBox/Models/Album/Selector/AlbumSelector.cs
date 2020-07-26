using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Box;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.History;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Album.Selector {
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
		public ReadOnlyReactiveCollection<RegisteredAlbumObject> AlbumList {
			get;
		}

		/// <summary>
		/// アルバム
		/// </summary>
		public AlbumModel Album {
			get;
		}


		/// <summary>
		/// カレントアルバムオブジェクト
		/// </summary>
		private IReactiveProperty<IAlbumObject> AlbumObject {
			get;
		} = new ReactiveProperty<IAlbumObject>();

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
			DocumentDb documentDb,
			AlbumHistoryManager albumHistoryManager,
			FilterDescriptionManager filterSetter,
			SortDescriptionManager sortSetter,
			MediaBoxDbContext rdb,
			MediaFileManager mediaFileManager,
			AlbumModel albumModel) {
			this._rdb = rdb;
			this._albumContainer = albumContainer;
			this.FilterSetter = filterSetter;
			this.SortSetter = sortSetter;
			this.Album = albumModel;
			this._documentDb = documentDb;
			// アルバムIDリストからアルバムリストの生成
			this.AlbumList = this._albumContainer.AlbumList.ToReadOnlyReactiveCollection(x => new RegisteredAlbumObject { AlbumId = x }).AddTo(this.CompositeDisposable);

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

			this.OnDisposed
				.Subscribe(_ => {
					this.Album.Dispose();
				}).AddTo(this.CompositeDisposable);

			this.AlbumObject.Where(x => x != null).Subscribe(x => {
				// TODO: タイトルをつける
				albumHistoryManager.Add(null, x);
				this.Album.SetAlbum(x);
			}).AddTo(this.CompositeDisposable);

			this.FilterSetter.OnFilteringConditionChanged
				.Merge(this.SortSetter.OnSortConditionChanged)
				.Subscribe(_ => {
					this.Album.LoadMediaFiles();
				});
		}

		/// <summary>
		/// 引数のアルバムをカレントする
		/// </summary>
		/// <param name="album"></param>
		public void SetAlbumToCurrent(IAlbumObject albumObject) {
			this.AlbumObject.Value = albumObject;
		}

		/// <summary>
		/// フォルダアルバムをカレントにする
		/// </summary>
		public void SetFolderAlbumToCurrent(string path) {
			if (path == null) {
				return;
			}
			var fao = new FolderAlbumObject { FolderPath = path };
			this.AlbumObject.Value = fao;
		}

		/// <summary>
		/// データベース検索アルバムをカレントにする
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public void SetDatabaseAlbumToCurrent(string tagName) {
			var ldao = new LookupDatabaseAlbumObject {
				TagName = tagName
			};
			this.AlbumObject.Value = ldao;
		}

		/// <summary>
		/// ワード検索アルバムをカレントにする
		/// </summary>
		/// <param name="word">検索ワード</param>
		public void SetWordSearchAlbumToCurrent(string word) {
			var ldao = new LookupDatabaseAlbumObject {
				Word = word
			};
			this.AlbumObject.Value = ldao;
		}

		/// <summary>
		/// 場所検索アルバムをカレントにする
		/// </summary>
		/// <param name="address">場所情報</param>
		public void SetPositionSearchAlbumToCurrent(Address address) {
			var ldao = new LookupDatabaseAlbumObject {
				Address = address
			};
			this.AlbumObject.Value = ldao;
		}

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="album">削除対象アルバム</param>
		public void DeleteAlbum(IAlbumObject albumObject) {
			if (albumObject is not RegisteredAlbumObject rao) {
				throw new ArgumentException();
			}
			lock (this._rdb) {
				this._rdb.Remove(this._rdb.Albums.Single(x => x.AlbumId == rao.AlbumId));
				this._rdb.SaveChanges();
			}
			this._albumContainer.RemoveAlbum(rao.AlbumId);
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
			return $"<[{base.ToString()}] {this.Album.Title.Value}>";
		}
	}

	/// <summary>
	/// メインウィンドウ用アルバムセレクター
	/// </summary>
	public class MainAlbumSelector : AlbumSelector {
		public MainAlbumSelector(
			AlbumContainer albumContainer,
			DocumentDb documentDb,
			AlbumHistoryManager albumHistoryManager,
			FilterDescriptionManager filterSetter,
			SortDescriptionManager sortSetter,
			MediaBoxDbContext rdb,
			MediaFileManager mediaFileManager,
			AlbumModel albumModel)
			: base(albumContainer, documentDb, albumHistoryManager, filterSetter, sortSetter, rdb, mediaFileManager, albumModel) {
			this.SetName("main");
		}
	}

	/// <summary>
	/// 編集ウィンドウ用アルバムセレクター
	/// </summary>
	public class EditorAlbumSelector : AlbumSelector {
		public EditorAlbumSelector(
			AlbumContainer albumContainer,
			DocumentDb documentDb,
			AlbumHistoryManager albumHistoryManager,
			FilterDescriptionManager filterSetter,
			SortDescriptionManager sortSetter,
			MediaBoxDbContext rdb,
			MediaFileManager mediaFileManager,
			AlbumModel albumModel)
			: base(albumContainer, documentDb, albumHistoryManager, filterSetter, sortSetter, rdb, mediaFileManager, albumModel) {
			this.SetName("editor");
		}
	}
}
