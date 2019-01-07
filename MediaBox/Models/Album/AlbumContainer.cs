using System;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバムコンテナ
	/// </summary>
	internal class AlbumContainer : ModelBase {
		/// <summary>
		/// アルバム一覧
		/// </summary>
		public ReactiveCollection<RegisteredAlbum> AlbumList {
			get;
		} = new ReactiveCollection<RegisteredAlbum>();

		/// <summary>
		/// アルバム格納棚
		/// </summary>
		public ReactivePropertySlim<AlbumBox> Shelf {
			get;
		} = new ReactivePropertySlim<AlbumBox>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumContainer() {
			// 初期値
			this.Shelf.Value = Get.Instance<AlbumBox>("root", "", this.AlbumList).AddTo(this.CompositeDisposable);

			// アルバムリストへ追加,アルバムリスト内のアルバムパス変化時
			this.AlbumList.ToReadOnlyReactiveCollection(x => {
				return x.AlbumPath.Subscribe(_ => {
					this.Shelf.Value.Update(this.AlbumList);
				});
			}).AddTo(this.CompositeDisposable);

			// アルバムリストから削除時
			this.AlbumList.ObserveRemoveChanged().Subscribe(x => {
				this.Shelf.Value.Update(this.AlbumList);
			});

			// アルバムリスト初期読み込み
			this.AlbumList.AddRange(
				this.DataBase
					.Albums
					.Select(x => x.AlbumId)
					.ToList()
					.Select(x => {
						var ra = Get.Instance<RegisteredAlbum>();
						ra.LoadFromDataBase(x);
						return ra;
					}));
		}

		/// <summary>
		/// アルバム追加
		/// </summary>
		/// <param name="album">追加対象アルバム</param>
		public void AddAlbum(RegisteredAlbum album) {
			this.AlbumList.Add(album);
		}

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="album">削除対象アルバム</param>
		public void RemoveAlbum(RegisteredAlbum album) {
			this.AlbumList.Remove(album);
			album.Dispose();
		}

	}
}
