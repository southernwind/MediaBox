using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Media {
	internal class MediaFileCollectionViewModel : MediaFileCollectionViewModel<MediaFileCollection> {
		public MediaFileCollectionViewModel() : base(Get.Instance<MediaFileCollection>()) {

		}
	}

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

			this.Items = mediaFileCollection.Items.ToReadOnlyReactiveCollection(x => Get.Instance<MediaFileViewModel>(x)).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// リストにメディアファイルVM追加
		/// </summary>
		/// <param name="mediaFileViewModel"></param>
		public void Add(MediaFileViewModel mediaFileViewModel) {
			this.Model.Items.Add(mediaFileViewModel.Model);
		}

		/// <summary>
		/// リストからメディアファイルVM削除
		/// </summary>
		/// <param name="mediaFileViewModel"></param>
		public void Remove(MediaFileViewModel mediaFileViewModel) {
			this.Model.Items.Remove(mediaFileViewModel.Model);
		}
	}
}
