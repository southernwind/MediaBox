using System.Linq;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバムコンテナ
	/// </summary>
	/// <remarks>
	/// DIコンテナによってSingletonとして扱われる。
	/// 生成されているすべての<see cref="RegisteredAlbum"/>を保持する。
	/// </remarks>
	internal class AlbumContainer : ModelBase {
		/// <summary>
		/// アルバムリスト
		/// </summary>
		public ReactiveCollection<int> AlbumList {
			get;
		} = new ReactiveCollection<int>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumContainer() {
			lock (this.DataBase) {
				// アルバムリスト初期読み込み
				this.AlbumList.AddRange(this.DataBase.Albums.Select(x => x.AlbumId));
			}
		}

		/// <summary>
		/// アルバム追加
		/// </summary>
		/// <param name="album">追加対象アルバム</param>
		public void AddAlbum(int albumId) {
			this.AlbumList.Add(albumId);
		}

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="album">削除対象アルバム</param>
		public void RemoveAlbum(int albumId) {
			this.AlbumList.Remove(albumId);
		}

	}
}
