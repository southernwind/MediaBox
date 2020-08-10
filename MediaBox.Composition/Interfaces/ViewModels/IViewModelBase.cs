using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;

using SandBeige.MediaBox.Composition.Enum;

namespace SandBeige.MediaBox.Composition.Interfaces.ViewModels {
	public interface IViewModelBase : INotifyPropertyChanged, IDisposable {
		/// <summary>
		/// Dispose済みか
		/// </summary>
		DisposeState DisposeState {
			get;
		}

		/// <summary>
		/// Dispose通知
		/// </summary>
		IObservable<Unit> OnDisposed {
			get;
		}

		/// <summary>
		/// まとめてDispose
		/// </summary>
		CompositeDisposable CompositeDisposable {
			get;
		}
	}
}
