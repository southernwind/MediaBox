using System;
using System.Linq;
using System.Reactive;
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

			// TODO : AlbumList内のPathの変化時にも作成し直す必要がある
			// TODO : 開いていた棚が再作成時に閉じてしまうので、本当は再作成ではなく編集をしなければならない
			this.AlbumList.ToCollectionChanged().ToUnit()
				.Merge(Observable.Return(Unit.Default)).Subscribe(_ => {
					this.Shelf.Value = Get.Instance<AlbumBox>("root", "", this.AlbumList).AddTo(this.CompositeDisposable);
				});
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
