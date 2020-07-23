using System.Collections.ObjectModel;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.DataBase;

namespace SandBeige.MediaBox.Models.Album {
	public class AlbumBoxSelector : ModelBase {
		/// <summary>
		/// ルートアルバムボックス
		/// </summary>
		public IReactiveProperty<AlbumBox> Shelf {
			get;
		} = new ReactivePropertySlim<AlbumBox>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumBoxSelector(MediaBoxDbContext rdb) {
			// 初期値
			this.Shelf.Value = new AlbumBox(new ObservableCollection<RegisteredAlbum>().ToReadOnlyReactiveCollection(), rdb).AddTo(this.CompositeDisposable);
		}
	}
}
