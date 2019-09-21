using System;
using System.Windows.Input;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces {
	/// <summary>
	/// キー操作受信
	/// </summary>
	/// <remarks>
	/// コマンドを通して受信したイベントをIObservableで配信する。
	/// </remarks>
	public interface IGestureReceiver {
		/// <summary>
		/// キーイベント
		/// </summary>
		IObservable<KeyEventArgs> KeyEvent {
			get;
		}

		/// <summary>
		/// マウスイベント
		/// </summary>
		IObservable<MouseEventArgs> MouseEvent {
			get;
		}

		/// <summary>
		/// マウスホイールイベント
		/// </summary>
		IObservable<MouseWheelEventArgs> MouseWheelEvent {
			get;
		}

		/// <summary>
		/// Controlキーが押下状態か否か
		/// </summary>
		bool IsControlKeyPressed {
			get;
		}

		/// <summary>
		/// Shiftキーが押下状態か否か
		/// </summary>
		bool IsShiftKeyPressed {
			get;
		}

		/// <summary>
		/// Altキーが押下状態か否か
		/// </summary>
		bool IsAltKeyPressed {
			get;
		}

		/// <summary>
		/// キーイベントコマンド
		/// </summary>
		ReactiveCommand<KeyEventArgs> KeyEventCommand {
			get;
		}

		/// <summary>
		/// マウスイベントコマンド
		/// </summary>
		ReactiveCommand<MouseEventArgs> MouseEventCommand {
			get;
		}

		/// <summary>
		/// マウスホイールイベントコマンド
		/// </summary>
		ReactiveCommand<MouseWheelEventArgs> MouseWheelEventCommand {
			get;
		}
	}
}
