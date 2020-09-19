
using System.Reactive.Disposables;

using SandBeige.MediaBox.Composition.Interfaces.Services;

namespace SandBeige.MediaBox.Composition.Bases {
	public class ServiceBase : IServiceBase {
		/// <summary>
		/// まとめてDispose
		/// </summary>
		private CompositeDisposable? _compositeDisposable;

		/// <summary>
		/// まとめてDispose
		/// </summary>
		public CompositeDisposable CompositeDisposable {
			get {
				return this._compositeDisposable ??= new();
			}
		}
	}
}
