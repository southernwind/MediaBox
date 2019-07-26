using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.History;
using SandBeige.MediaBox.Models.Album.History.Creator;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバム選択
	/// </summary>
	/// <remarks>
	/// <see cref="AlbumContainer"/>に含まれているアルバムと、指定フォルダから生成した<see cref="FolderAlbum"/>のうちから
	/// 一つの<see cref="AlbumModel"/>を<see cref="CurrentAlbum"/>として選ぶ。
	/// <see cref="FolderAlbum"/>の場合はカレントでなくなった時点で<see cref="IDisposable.Dispose"/>される。
	/// </remarks>
	internal class AlbumSelector : ModelBase, IAlbumSelector {
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
		/// アルバムフォルダパス
		/// </summary>
		public IReactiveProperty<string> FolderAlbumPath {
			get;
		} = new ReactiveProperty<string>();

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
		/// <param name="name">一意になる名称 フィルターとソート順の保存、復元に使用する。</param>
		public AlbumSelector(string name) {
			this._albumContainer = Get.Instance<AlbumContainer>();

			this.FilterSetter = new FilterDescriptionManager(name);
			this.SortSetter = new SortDescriptionManager(name);

			// アルバムIDリストからアルバムリストの生成
			this.AlbumList = this._albumContainer.AlbumList.ToReadOnlyReactiveCollection(x => {
				var ra = new RegisteredAlbum(this);
				ra.LoadFromDataBase(x);
				return ra;
			}).AddTo(this.CompositeDisposable);

			// 初期値
			this.Shelf.Value = new AlbumBox(this.AlbumList).AddTo(this.CompositeDisposable);

			IEnumerable<ValueCountPair<string>> func() {
				lock (this.DataBase) {
					return this.DataBase
						.MediaFiles
						.GroupBy(x => x.DirectoryPath)
						.Select(x => new ValueCountPair<string>(x.Key, x.Count()))
						.ToList();
				}
			}

			this.Folder.Value = new FolderObject("", func());

			Get.Instance<MediaFileManager>()
					.OnRegisteredMediaFiles
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

			var albumHistoryManager = Get.Instance<AlbumHistoryManager>();
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
			this.CurrentAlbum.Value = albumCreator?.Create(this);
		}

		/// <summary>
		/// フォルダアルバムをカレントにする
		/// </summary>
		public void SetFolderAlbumToCurrent() {
			if (this.FolderAlbumPath.Value == null) {
				return;
			}
			var album = new FolderAlbum(this.FolderAlbumPath.Value, this);
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// データベース検索アルバムをカレントにする
		/// </summary>
		/// <param name="albumTitle">アルバムタイトル</param>
		/// <param name="tagName">タグ名</param>
		public void SetDatabaseAlbumToCurrent(string albumTitle, string tagName) {
			var album = new LookupDatabaseAlbum(this);
			album.Title.Value = albumTitle;
			album.TagName = tagName;
			album.LoadFromDataBase();
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// ワード検索アルバムをカレントにする
		/// </summary>
		/// <param name="albumTitle">アルバムタイトル</param>
		/// <param name="word">検索ワード</param>
		public void WordSearchAlbumToCurrent(string albumTitle, string word) {
			var album = new LookupDatabaseAlbum(this);
			album.Title.Value = albumTitle;
			album.Word = word;
			album.LoadFromDataBase();
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// 場所検索アルバムをカレントにする
		/// </summary>
		/// <param name="albumTitle">アルバムタイトル</param>
		/// <param name="address">場所情報</param>
		public void PositionSearchAlbumToCurrent(string albumTitle, Address address) {
			var album = new LookupDatabaseAlbum(this);
			album.Title.Value = albumTitle;
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
				return;
			}
			lock (this.DataBase) {
				this.DataBase.Remove(this.DataBase.Albums.Single(x => x.AlbumId == ra.AlbumId.Value));
				this.DataBase.SaveChanges();
			}
			this._albumContainer.RemoveAlbum(ra.AlbumId.Value);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.CurrentAlbum.Value?.Title.Value}>";
		}
	}
}
