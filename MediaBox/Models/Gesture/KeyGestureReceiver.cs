using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Models.Gesture {
	/// <summary>
	/// キー操作受信
	/// </summary>
	/// <remarks>
	/// コマンドを通して受信したキーイベントをIObservableで配信する。
	/// </remarks>
	internal class KeyGestureReceiver {
		private readonly Subject<KeyEventArgs> _keyEventSubject = new Subject<KeyEventArgs>();

		/// <summary>
		/// キーイベント
		/// </summary>
		public IObservable<KeyEventArgs> KeyEvent {
			get {
				return this._keyEventSubject.AsObservable();
			}
		}

		/// <summary>
		/// キーイベントコマンド
		/// </summary>
		public ReactiveCommand<KeyEventArgs> KeyEventCommand {
			get;
		} = new ReactiveCommand<KeyEventArgs>();

		public KeyGestureReceiver() {
			this.KeyEventCommand.Subscribe(e => {
				this._keyEventSubject.OnNext(e);
			});
		}
	}
}
