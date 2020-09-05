using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Map {
	/// <summary>
	/// マップピンViewModel
	/// このグループを一つのピンとして表示する
	/// </summary>
	public class MapPointerViewModel : MediaFileCollectionViewModel<IMapPointer> {
		/// <summary>
		/// 代表ファイル
		/// </summary>
		public IReadOnlyReactiveProperty<IMediaFileViewModel?> Core {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public MapPointerViewModel(IMapPointer model, ViewModelFactory viewModelFactory) : base(model, viewModelFactory) {
			this.Core =
				this.Model
					.Core
					.Select(viewModelFactory.Create)
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);
		}
	}
}
