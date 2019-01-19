using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Map {
	/// <summary>
	/// マップピンViewModel
	/// このグループを一つのピンとして表示する
	/// </summary>
	internal class MapPinViewModel : MediaFileCollectionViewModel<MapPin> {
		/// <summary>
		/// 代表ファイル
		/// </summary>
		public IReadOnlyReactiveProperty<MediaFileViewModel> Core {
			get;
		}

		/// <summary>
		/// ピン状態
		/// </summary>
		public IReadOnlyReactiveProperty<PinState> PinState {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public MapPinViewModel(MapPin model) : base(model) {
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
