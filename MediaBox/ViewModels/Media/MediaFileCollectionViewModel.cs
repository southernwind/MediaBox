using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// メディアファイルコレクションViewModel基底クラス
	/// </summary>
	/// <typeparam name="T">Model型</typeparam>
	internal class MediaFileCollectionViewModel<T> : ViewModelBase
		where T : MediaFileCollection {
		/// <summary>
		/// メディアファイルコレクションModel
		/// </summary>
		public T Model {
			get;
		}

		/// <summary>
		/// 件数
		/// </summary>
		public ReadOnlyReactivePropertySlim<int> Count {
			get;
		}

		/// <summary>
		/// メディアファイルViewModelリスト
		/// </summary>
		public ReadOnlyReactiveCollection<MediaFileViewModel> Items {
			get;
		}

		public MediaFileCollectionViewModel(T mediaFileCollection) {
			this.Model = mediaFileCollection;

			this.Count = mediaFileCollection.Count.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.Items = mediaFileCollection.Items.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create, disposeElement: false).AddTo(this.CompositeDisposable);
		}
	}
}
