using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// メディアファイルコレクションViewModel基底クラス
	/// </summary>
	/// <typeparam name="T">Model型</typeparam>
	internal abstract class MediaFileCollectionViewModel<T> : ViewModelBase
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
		public IReadOnlyReactiveProperty<int> Count {
			get;
		}

		/// <summary>
		/// メディアファイルViewModelリスト
		/// </summary>
		public ReadOnlyReactiveCollection<IMediaFileViewModel> Items {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mediaFileCollection">モデルインスタンス</param>
		protected MediaFileCollectionViewModel(T mediaFileCollection) {
			this.Model = mediaFileCollection;
			this.ModelForToString = this.Model;

			this.Count = mediaFileCollection.Count.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.Items = mediaFileCollection.Items.Lock(x => x.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create, disposeElement: false)).AddTo(this.CompositeDisposable);

			// モデル破棄時にこのインスタンスも破棄
			this.AddTo(mediaFileCollection.CompositeDisposable);
		}
	}
}
