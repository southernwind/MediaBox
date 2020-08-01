
using Reactive.Bindings;
using Reactive.Bindings.Helpers;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box {
	public interface IAlbumBox {

		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReactiveProperty<int?> AlbumBoxId {
			get;
		}

		/// <summary>
		/// アルバムボックスタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		}

		/// <summary>
		/// 子アルバムボックス
		/// </summary>
		public ReactiveCollection<IAlbumBox> Children {
			get;
		}

		/// <summary>
		/// 直下アルバム
		/// 子アルバムボックスのアルバムはここには含まれない
		/// </summary>
		public IFilteredReadOnlyObservableCollection<IAlbumForBoxModel> Albums {
			get;
		}

		/// <summary>
		/// 子アルバムボックス追加
		/// </summary>
		/// <param name="name"></param>
		public void AddChild(string name);

		/// <summary>
		/// アルバムボックス削除
		/// </summary>
		public void Remove();

		/// <summary>
		/// アルバムボックスタイトル変更
		/// </summary>
		/// <param name="name">変更後タイトル</param>
		public void Rename(string name);
	}
}
