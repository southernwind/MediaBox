using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Container;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.History;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Loader;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Box;
using SandBeige.MediaBox.Models.Album.Filter;

namespace SandBeige.MediaBox.Models.Album.Selector {
	/// <summary>
	/// アルバム選択
	/// </summary>
	public class AlbumSelector : ModelBase, IAlbumSelector {
		private readonly IAlbumObjectCreator _albumObjectCreator;
		/// <summary>
		/// コンテナ
		/// </summary>
		private readonly IAlbumContainer _albumContainer;

		/// <summary>
		/// フィルター
		/// </summary>
		public IFilterDescriptionManager FilterSetter {
			get;
		}

		/// <summary>
		/// ソート
		/// </summary>
		public ISortDescriptionManager SortSetter {
			get;
		}

		/// <summary>
		/// アルバム
		/// </summary>
		public IAlbumModel Album {
			get;
		}


		/// <summary>
		/// カレントアルバムオブジェクト
		/// </summary>
		private IReactiveProperty<IAlbumObject?> AlbumObject {
			get;
		} = new ReactiveProperty<IAlbumObject?>();

		/// <summary>
		/// ルートアルバムボックス
		/// </summary>
		public IReactiveProperty<IAlbumBox> Shelf {
			get;
		} = new ReactivePropertySlim<IAlbumBox>();

		/// <summary>
		/// Folder
		/// </summary>
		public IReactiveProperty<IAlbumSelectorFolderObject> Folder {
			get;
		} = new ReactivePropertySlim<IAlbumSelectorFolderObject>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="albumContainer">アルバムコンテナ</param>
		/// <param name="albumHistoryManager">アルバム履歴管理</param>
		/// <param name="filterSetter">フィルターマネージャー</param>
		/// <param name="sortSetter">ソートマネージャー</param>
		public AlbumSelector(
			IAlbumContainer albumContainer,
			IDocumentDb documentDb,
			IAlbumHistoryRegistry albumHistoryManager,
			IFilterDescriptionManager filterSetter,
			ISortDescriptionManager sortSetter,
			IMediaBoxDbContext rdb,
			IMediaFileManager mediaFileManager,
			IAlbumModel albumModel,
			IAlbumObjectCreator albumObjectCreator,
			IAlbumLoaderFactory albumLoaderFactory) {
			this._albumContainer = albumContainer;
			this.FilterSetter = filterSetter;
			this.SortSetter = sortSetter;
			this.Album = albumModel;
			this._albumObjectCreator = albumObjectCreator;
			this.Album.SetFilterAndSort(filterSetter, sortSetter);

			// アルバムIDリストからアルバムリストの生成
			var albumList = this._albumContainer.AlbumList.ToReadOnlyReactiveCollection(x => new RegisteredAlbumObject { AlbumId = x }).AddTo(this.CompositeDisposable);

			// 初期値
			this.Shelf.Value = new AlbumBox(albumList, rdb, albumLoaderFactory).AddTo(this.CompositeDisposable);

			IEnumerable<ValueCountPair<string>> Func() {
				lock (rdb) {
					return documentDb
						.GetMediaFilesCollection()
						.Query()
						// TODO : 暫定対応
						.ToEnumerable()
						.GroupBy(x => x.DirectoryPath)
						.Select(x => new ValueCountPair<string>(x.Key, x.Count()))
						.ToList();
				}
			}

			this.Folder.Value = new FolderObject("", Func());

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
						this.Folder.Value.Update(Func());
					}
				});

			this.OnDisposed
				.Subscribe(_ => {
					this.Album.Dispose();
				}).AddTo(this.CompositeDisposable);

			this.AlbumObject.Where(x => x != null).Subscribe(x => {
				this.Album.SetAlbum(x!);
				albumHistoryManager.Add(this.Album.Title.Value, x!);
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
		public void SetAlbumToCurrent(IAlbumObject? albumObject) {
			this.AlbumObject.Value = albumObject;
		}

		/// <summary>
		/// フォルダアルバムをカレントにする
		/// </summary>
		public void SetFolderAlbumToCurrent(string path) {
			this.SetAlbumToCurrent(this._albumObjectCreator.CreateFolderAlbum(path));
		}

		/// <summary>
		/// データベース検索アルバムをカレントにする
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public void SetDatabaseAlbumToCurrent(string tagName) {
			this.SetAlbumToCurrent(this._albumObjectCreator.CreateDatabaseAlbum(tagName));
		}

		/// <summary>
		/// ワード検索アルバムをカレントにする
		/// </summary>
		/// <param name="word">検索ワード</param>
		public void SetWordSearchAlbumToCurrent(string word) {
			this.SetAlbumToCurrent(this._albumObjectCreator.CreateWordSearchAlbum(word));
		}

		/// <summary>
		/// 場所検索アルバムをカレントにする
		/// </summary>
		/// <param name="address">場所情報</param>
		public void SetPositionSearchAlbumToCurrent(IAddress address) {
			this.SetAlbumToCurrent(this._albumObjectCreator.CreatePositionSearchAlbum(address));
		}

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="album">削除対象アルバム</param>
		public void DeleteAlbum(IAlbumObject albumObject) {
			this._albumContainer.RemoveAlbum(albumObject);
		}

		/// <summary>
		/// 名称設定
		/// </summary>
		/// <param name="name">一意になる名称 フィルターとソート順の保存、復元に使用する。</param>
		public void SetName(string name) {
			this.FilterSetter.Name.Value = name;
			this.SortSetter.Name.Value = name;
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Album.Title.Value}>";
		}
	}
}
