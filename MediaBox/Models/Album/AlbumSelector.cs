using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.History;
using SandBeige.MediaBox.Models.Album.History.Creator;
using SandBeige.MediaBox.Models.Album.Sort;
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
	internal class AlbumSelector : ModelBase {
		/// <summary>
		/// コンテナ
		/// </summary>
		private readonly AlbumContainer _albumContainer;

		/// <summary>
		/// フィルターマネージャー
		/// </summary>
		public FilterDescriptionManager FilterDescriptionManager {
			get;
		}

		/// <summary>
		/// ソートマネージャー
		/// </summary>
		public SortDescriptionManager SortDescriptionManager {
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

			this.FilterDescriptionManager = Get.Instance<FilterDescriptionManager>(name);
			this.SortDescriptionManager = Get.Instance<SortDescriptionManager>(name);

			// アルバムIDリストからアルバムリストの生成
			this.AlbumList = this._albumContainer.AlbumList.ToReadOnlyReactiveCollection(x => {
				var ra = Get.Instance<RegisteredAlbum>(this.FilterDescriptionManager, this.SortDescriptionManager);
				ra.LoadFromDataBase(x);
				return ra;
			}).AddTo(this.CompositeDisposable);

			// 初期値
			this.Shelf.Value = Get.Instance<AlbumBox>("root", "", this.AlbumList).AddTo(this.CompositeDisposable);

			IEnumerable<ValueCountPair<string>> func() {
				lock (this.DataBase) {
					return this.DataBase
						.MediaFiles
						.GroupBy(x => x.DirectoryPath)
						.Select(x => new ValueCountPair<string>(x.Key, x.Count()))
						.ToList();
				}
			}

			this.Folder.Value = Get.Instance<FolderObject>("", func());

			Get.Instance<MediaFileManager>()
					.OnRegisteredMediaFiles
					.Throttle(TimeSpan.FromMilliseconds(100))
					.Synchronize()
					.ObserveOn(UIDispatcherScheduler.Default)
					.Subscribe(_ => this.Folder.Value.Update(func()));

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

			this.FilterDescriptionManager.OnFilteringConditionChanged
				.Merge(this.SortDescriptionManager.OnSortConditionChanged)
				.Throttle(TimeSpan.FromMilliseconds(100))
				.ObserveOn(TaskPoolScheduler.Default)
				.Synchronize()
				.Subscribe(_ => {
					// TODO : キャンセルの仕組みが必要か
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
		/// <param name="album"></param>
		public void SetAlbumToCurrent(IAlbumCreator albumCreator) {
			this.CurrentAlbum.Value = albumCreator.Create(this.FilterDescriptionManager, this.SortDescriptionManager);
		}

		/// <summary>
		/// フォルダアルバムをカレントにする
		/// </summary>
		public void SetFolderAlbumToCurrent() {
			if (this.FolderAlbumPath.Value == null) {
				return;
			}
			var album = Get.Instance<FolderAlbum>(this.FolderAlbumPath.Value, this.FilterDescriptionManager, this.SortDescriptionManager);
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// データベース検索アルバムをカレントにする
		/// </summary>
		/// <param name="albumTitle">アルバムタイトル</param>
		/// <param name="tagName">タグ名</param>
		public void SetDatabaseAlbumToCurrent(string albumTitle, string tagName) {
			var album = Get.Instance<LookupDatabaseAlbum>(this.FilterDescriptionManager, this.SortDescriptionManager);
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
