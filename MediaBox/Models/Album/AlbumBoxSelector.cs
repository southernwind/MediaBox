using System.Collections.ObjectModel;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	internal class AlbumBoxSelector : ModelBase {
		/// <summary>
		/// ルートアルバムボックス
		/// </summary>
		public IReactiveProperty<AlbumBox> Shelf {
			get;
		} = new ReactivePropertySlim<AlbumBox>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumBoxSelector() {
			// 初期値
			this.Shelf.Value = new AlbumBox(new ObservableCollection<RegisteredAlbum>().ToReadOnlyReactiveCollection()).AddTo(this.CompositeDisposable);
		}
	}
}
