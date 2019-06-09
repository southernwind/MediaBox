﻿using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Models.Gesture {
	/// <summary>
	/// キー操作受信
	/// </summary>
	/// <remarks>
	/// コマンドを通して受信したイベントをIObservableで配信する。
	/// </remarks>
	internal class GestureReceiver {
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
		public GestureReceiver() {
			this.KeyEventCommand.Subscribe(this._keyEventSubject.OnNext);
			this.MouseEventCommand.Subscribe(this._mouseEventSubject.OnNext);
			this.MouseWheelEventCommand.Subscribe(this._mouseWheelEventSubject.OnNext);
		}
	}
}