using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;

namespace SandBeige.MediaBox.ViewModels.Album.Box {
	public class AlbumForBoxViewModel : ViewModelBase {
		/// <summary>
		/// タイトル
		/// </summary>
		public IReadOnlyReactiveProperty<string> Title {
			get;
		}

		/// <summary>
		/// 件数
		/// </summary>
		public IReadOnlyReactiveProperty<int> Count {
			get;
		}

		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReadOnlyReactiveProperty<int?> AlbumBoxId {
			get;
		}

		/// <summary>
		/// アルバムオブジェクト
		/// </summary>
		public IAlbumObject AlbumObject {
			get;
		}

		public AlbumForBoxViewModel(IAlbumForBoxModel albumModelForBox) {
			this.AlbumObject = albumModelForBox.AlbumObject;
			this.ModelForToString = albumModelForBox;
			this.Title = albumModelForBox.Title.ToReadOnlyReactivePropertySlim(null!).AddTo(this.CompositeDisposable);
			this.Count = albumModelForBox.Count.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.AlbumBoxId = albumModelForBox.AlbumBoxId.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
		}
	}
}
