using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Gesture {
	/// <summary>
	/// キー操作受信
	/// </summary>
	/// <remarks>
	/// コマンドを通して受信したイベントをIObservableで配信する。
	/// </remarks>
	public class GestureReceiver : IGestureReceiver {
		private readonly Subject<KeyEventArgs> _keyEventSubject = new Subject<KeyEventArgs>();
		private readonly Subject<MouseEventArgs> _mouseEventSubject = new Subject<MouseEventArgs>();
		private readonly Subject<MouseWheelEventArgs> _mouseWheelEventSubject = new Subject<MouseWheelEventArgs>();

		/// <summary>
		/// キーイベント
		/// </summary>
		public IObservable<KeyEventArgs> KeyEvent {
			get {
				return this._keyEventSubject.AsObservable();
			}
		}

		/// <summary>
		/// マウスイベント
		/// </summary>
		public IObservable<MouseEventArgs> MouseEvent {
			get {
				return this._mouseEventSubject.AsObservable();
			}
		}

		/// <summary>
		/// マウスホイールイベント
		/// </summary>
		public IObservable<MouseWheelEventArgs> MouseWheelEvent {
			get {
				return this._mouseWheelEventSubject.AsObservable();
			}
		}

		/// <summary>
		/// Controlキーが押下状態か否か
		/// </summary>
		public bool IsControlKeyPressed {
			get {
				return
					Keyboard.GetKeyStates(Key.LeftCtrl).HasFlag(KeyStates.Down) ||
					Keyboard.GetKeyStates(Key.RightCtrl).HasFlag(KeyStates.Down);
			}
		}

		/// <summary>
		/// Shiftキーが押下状態か否か
		/// </summary>
		public bool IsShiftKeyPressed {
			get {
				return
					Keyboard.GetKeyStates(Key.LeftShift).HasFlag(KeyStates.Down) ||
					Keyboard.GetKeyStates(Key.RightShift).HasFlag(KeyStates.Down);
			}
		}

		/// <summary>
		/// Altキーが押下状態か否か
		/// </summary>
		public bool IsAltKeyPressed {
			get {
				return
					Keyboard.GetKeyStates(Key.LeftAlt).HasFlag(KeyStates.Down) ||
					Keyboard.GetKeyStates(Key.RightAlt).HasFlag(KeyStates.Down);
			}
		}

		/// <summary>
		/// キーイベントコマンド
		/// </summary>
		public ReactiveCommand<KeyEventArgs> KeyEventCommand {
			get;
		} = new ReactiveCommand<KeyEventArgs>();

		/// <summary>
		/// マウスイベントコマンド
		/// </summary>
		public ReactiveCommand<MouseEventArgs> MouseEventCommand {
			get;
		} = new ReactiveCommand<MouseEventArgs>();

		/// <summary>
		/// マウスホイールイベントコマンド
		/// </summary>
		public ReactiveCommand<MouseWheelEventArgs> MouseWheelEventCommand {
			get;
		} = new ReactiveCommand<MouseWheelEventArgs>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		[Obsolete("for DI container")]
		public GestureReceiver() {
			this.KeyEventCommand.Subscribe(this._keyEventSubject.OnNext);
			this.MouseEventCommand.Subscribe(this._mouseEventSubject.OnNext);
			this.MouseWheelEventCommand.Subscribe(this._mouseWheelEventSubject.OnNext);
		}
	}
}
