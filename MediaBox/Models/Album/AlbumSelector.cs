using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.History;
using SandBeige.MediaBox.Models.Album.History.Creator;
using SandBeige.MediaBox.Models.Album.Sort;
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
	internal class AlbumSelector : ModelBase {
		/// <summary>
		/// コンテナ
		/// </summary>
		private readonly AlbumContainer _albumContainer;

		private readonly FilterDescriptionManager _filterDescriptionManager;
		private readonly SortDescriptionManager _sortDescriptionManager;

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
		/// コンストラクタ
		/// </summary>
		public AlbumSelector() {
			this._albumContainer = Get.Instance<AlbumContainer>();

			this._filterDescriptionManager = Get.Instance<FilterDescriptionManager>();
			this._sortDescriptionManager = Get.Instance<SortDescriptionManager>();

			// アルバムIDリストからアルバムリストの生成
			this.AlbumList = this._albumContainer.AlbumList.ToReadOnlyReactiveCollection(x => {
				var ra = Get.Instance<RegisteredAlbum>(this._filterDescriptionManager, this._sortDescriptionManager);
				ra.LoadFromDataBase(x);
				return ra;
			}).AddTo(this.CompositeDisposable);

			// 初期値
			this.Shelf.Value = Get.Instance<AlbumBox>("root", "", this.AlbumList).AddTo(this.CompositeDisposable);

			// アルバムボックス更新
			this.AlbumList
				.ObserveElementObservableProperty(x => x.AlbumPath).ToUnit()
				.Merge(this.AlbumList.ObserveRemoveChanged().ToUnit())
				.Subscribe(_ => {
					this.Shelf.Value.Update(this.AlbumList);
				}).AddTo(this.CompositeDisposable);

			// アルバムリストから削除時
			this.AlbumList.ObserveRemoveChanged().Subscribe(x => {
				this.Shelf.Value.Update(this.AlbumList);
			});

			// カレントアルバム切り替え時、登録アルバム以外ならDisposeしておく
			this.CurrentAlbum
				.Pairwise()
				.Subscribe(x => {
					if (!(x.OldItem is RegisteredAlbum)) {
						x.OldItem?.Dispose();
					}
				}).AddTo(this.CompositeDisposable);

			var albumHistoryManager = Get.Instance<AlbumHistoryManager>();
			this.CurrentAlbum.Where(x => x != null).Subscribe(albumHistoryManager.Add).AddTo(this.CompositeDisposable);

			this._filterDescriptionManager.OnFilteringConditionChanged
				.Merge(this._sortDescriptionManager.OnSortConditionChanged)
				.Throttle(TimeSpan.FromMilliseconds(100))
				.ObserveOn(TaskPoolScheduler.Default)
				.Synchronize()
				.Subscribe(_ => {
					// TODO : キャンセルの仕組みが必要か
					this.CurrentAlbum.Value.Load();
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
		/// <param name="album"></param>
		public void SetAlbumToCurrent(IAlbumCreator albumCreator) {
			this.CurrentAlbum.Value = albumCreator.Create(this._filterDescriptionManager, this._sortDescriptionManager);
		}

		/// <summary>
		/// フォルダアルバムをカレントにする
		/// </summary>
		public void SetFolderAlbumToCurrent() {
			if (this.FolderAlbumPath.Value == null) {
				return;
			}
			var album = Get.Instance<FolderAlbum>(this.FolderAlbumPath.Value, this._filterDescriptionManager, this._sortDescriptionManager);
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// データベース検索アルバムをカレントにする
		/// </summary>
		/// <param name="albumTitle">アルバムタイトル</param>
		/// <param name="tagName">タグ名</param>
		public void SetDatabaseAlbumToCurrent(string albumTitle, string tagName) {
			var album = Get.Instance<LookupDatabaseAlbum>(this._filterDescriptionManager, this._sortDescriptionManager);
			album.Title.Value = albumTitle;
			album.TagName = tagName;
			album.LoadFromDataBase();
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="album">削除対象アルバム</param>
		public void DeleteAlbum(RegisteredAlbum album) {
			lock (this.DataBase) {
				this.DataBase.Remove(this.DataBase.Albums.Single(x => x.AlbumId == album.AlbumId.Value));
				this.DataBase.SaveChanges();
			}
			this._albumContainer.RemoveAlbum(album.AlbumId.Value);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.CurrentAlbum.Value?.Title.Value}>";
		}
	}
}
