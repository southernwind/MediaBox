using System.Linq;

using Reactive.Bindings;

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
		public ReactiveCollection<Album> AlbumList {
			get;
		} = new ReactiveCollection<Album>();

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
		}

		/// <summary>
		/// アルバム追加
		/// </summary>
		/// <param name="album">追加対象アルバム</param>
		public void AddAlbum(Album album) {
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
