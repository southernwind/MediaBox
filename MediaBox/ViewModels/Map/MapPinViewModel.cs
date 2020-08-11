using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Map {
	/// <summary>
	/// マップピンViewModel
	/// このグループを一つのピンとして表示する
	/// </summary>
	public class MapPinViewModel : MediaFileCollectionViewModel<IMapPin> {
		/// <summary>
		/// 代表ファイル
		/// </summary>
		public IReadOnlyReactiveProperty<IMediaFileViewModel> Core {
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
		public MapPinViewModel(IMapPin model, ViewModelFactory viewModelFactory) : base(model, viewModelFactory) {
			this.Core =
				this.Model
					.Core
					.Select(viewModelFactory.Create)
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);

			this.PinState = this.Model.PinState.ToReadOnlyReactivePropertySlim();
		}
	}
}
