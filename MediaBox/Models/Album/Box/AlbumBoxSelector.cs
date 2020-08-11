using System.Collections.ObjectModel;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
namespace SandBeige.MediaBox.Models.Album.Box {
	public class AlbumBoxSelector : ModelBase, IAlbumBoxSelector {
		/// <summary>
		/// ルートアルバムボックス
		/// </summary>
		public IReactiveProperty<IAlbumBox> Shelf {
			get;
		} = new ReactivePropertySlim<IAlbumBox>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumBoxSelector(IMediaBoxDbContext rdb) {
			// 初期値
			this.Shelf.Value = new AlbumBox(new ObservableCollection<RegisteredAlbumObject>().ToReadOnlyReactiveCollection(), rdb).AddTo(this.CompositeDisposable);
		}
	}
}
