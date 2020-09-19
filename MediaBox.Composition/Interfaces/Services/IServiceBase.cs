using System.Reactive.Disposables;

namespace SandBeige.MediaBox.Composition.Interfaces.Services {
	public interface IServiceBase {
		/// <summary>
		/// まとめてDispose
		/// </summary>
		CompositeDisposable CompositeDisposable {
			get;
		}
	}
}
