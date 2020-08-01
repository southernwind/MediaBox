using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Box;
namespace SandBeige.MediaBox.ViewModels.Album.Box {
	public class AlbumForBoxViewModel : ViewModelBase {
		/// <summary>
		/// タイトル
		/// </summary>
		public IReadOnlyReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 件数
		/// </summary>
		public IReadOnlyReactiveProperty<int> Count {
			get;
		} = new ReactivePropertySlim<int>();

		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReadOnlyReactiveProperty<int?> AlbumBoxId {
			get;
		} = new ReactivePropertySlim<int?>();

		/// <summary>
		/// アルバムオブジェクト
		/// </summary>
		public IAlbumObject AlbumObject {
			get;
		}

		public AlbumForBoxViewModel(AlbumForBoxModel albumModelForBox) {
			this.AlbumObject = albumModelForBox.AlbumObject;
			this.ModelForToString = albumModelForBox;
			this.Title = albumModelForBox.Title.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Count = albumModelForBox.Count.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.AlbumBoxId = albumModelForBox.AlbumBoxId.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
		}
	}
}
