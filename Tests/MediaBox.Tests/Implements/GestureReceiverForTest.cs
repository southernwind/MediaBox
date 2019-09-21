using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Input;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Tests.Implements {
	[Apartment(ApartmentState.STA)]
	internal class GestureReceiverForTest : IGestureReceiver {
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
			get;
			set;
		}

		/// <summary>
		/// Shiftキーが押下状態か否か
		/// </summary>
		public bool IsShiftKeyPressed {
			get;
			set;
		}

		/// <summary>
		/// Altキーが押下状態か否か
		/// </summary>
		public bool IsAltKeyPressed {
			get;
			set;
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
		public GestureReceiverForTest() {
			this.KeyEventCommand.Subscribe(this._keyEventSubject.OnNext);
			this.MouseEventCommand.Subscribe(this._mouseEventSubject.OnNext);
			this.MouseWheelEventCommand.Subscribe(this._mouseWheelEventSubject.OnNext);
		}
	}
}
