using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Models.Gesture {
	/// <summary>
	/// キー操作受信
	/// </summary>
	internal class KeyGestureReceiver {
		private readonly Subject<KeyEventArgs> _keyPressedSubject = new Subject<KeyEventArgs>();

		public IObservable<KeyEventArgs> KeyPressed {
			get {
				return this._keyPressedSubject.AsObservable();
			}
		}

		public ReactiveCommand<KeyEventArgs> KeyEventCommand {
			get;
		} = new ReactiveCommand<KeyEventArgs>();

		public KeyGestureReceiver() {
			this.KeyEventCommand.Subscribe(e => {
				this._keyPressedSubject.OnNext(e);
			});
		}

		public void OnKeyUp(KeyEventArgs e) {
			this._keyPressedSubject.OnNext(e);
			e.Handled = true;
		}

		public void OnKeyDown(KeyEventArgs e) {
			this._keyPressedSubject.OnNext(e);
			e.Handled = true;
		}
	}
}
