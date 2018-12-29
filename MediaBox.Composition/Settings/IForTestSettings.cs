using System;
using System.ComponentModel;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Settings {
	public interface IForTestSettings : INotifyPropertyChanged, IDisposable {
		IReactiveProperty<bool> RunOnBackground {
			get;
		}
	}
}
