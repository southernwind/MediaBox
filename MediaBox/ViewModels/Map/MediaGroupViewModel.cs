using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Map {
	/// <summary>
	/// マップ用メディアグループViewModel
	/// このグループを一つのピンとして表示する
	/// </summary>
	internal class MediaGroupViewModel : MediaFileCollectionViewModel<MediaGroup> {
		/// <summary>
		/// 代表ファイル
		/// </summary>
		public ReadOnlyReactivePropertySlim<MediaFileViewModel> Core {
			get;
		}

		/// <summary>
		/// ピン状態
		/// </summary>
		public ReadOnlyReactivePropertySlim<PinState> PinState {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public MediaGroupViewModel(MediaGroup model) : base(model) {
			this.Core =
				this.Model
					.Core
					.Select(this.ViewModelFactory.Create)
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);

			this.PinState = this.Model.PinState.ToReadOnlyReactivePropertySlim();
		}
	}
}
