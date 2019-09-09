using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Interop;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Gesture;
using SandBeige.MediaBox.Tests.Implements;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Gesture {
	[Apartment(ApartmentState.STA)]
	internal class GestureReceiverTest : ModelTestClassBase {
		[Test]
		public void キーイベント() {
#pragma warning disable 618
			var gr = new GestureReceiver();
#pragma warning restore 618
			var result = new List<Key>();
			gr.KeyEvent.Subscribe(x => result.Add(x.Key));
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.DbeCodeInput));
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.G));
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.F16));
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.Return));

			result.Is(Key.DbeCodeInput, Key.G, Key.F16, Key.Return);
		}

		[Test]
		public void マウスホイールイベント() {
#pragma warning disable 618
			var gr = new GestureReceiver();
#pragma warning restore 618
			var result = new List<int>();
			gr.MouseWheelEvent.Subscribe(x => result.Add(x.Delta));
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(-10));
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(50));
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(110));

			result.Select(x => x).Is(10, -10, 50, 110);
		}

		[Test]
		public void マウスイベント() {
#pragma warning disable 618
			var gr = new GestureReceiver();
#pragma warning restore 618
			var result = new List<MouseEventArgs>();
			var m1 = this.GetMouseEventArgs();
			var m2 = this.GetMouseEventArgs();
			var m3 = this.GetMouseEventArgs();

			gr.MouseEvent.Subscribe(result.Add);
			gr.MouseEventCommand.Execute(m1);
			gr.MouseEventCommand.Execute(m2);
			gr.MouseEventCommand.Execute(m3);

			result.Select(x => x).Is(m1, m2, m3);
		}

		[Test]
		public void プロパティ() {
#pragma warning disable 618
			var gr = new GestureReceiver();
#pragma warning restore 618
			gr.IsControlKeyPressed.IsFalse();
			gr.IsShiftKeyPressed.IsFalse();
			gr.IsAltKeyPressed.IsFalse();
		}

		/// <summary>
		/// キーを指定して<see cref="KeyEventArgs"/>のインスタンスを生成します。
		/// </summary>
		/// <param name="key">キー</param>
		/// <returns><see cref="KeyEventArgs"/>インスタンス</returns>
		private KeyEventArgs GetKeyEventArgs(Key key) {
			var kbd = new KeyboardDeviceForTest(InputManager.Current) {
				DownKeys = new[] { key }
			};
			return new KeyEventArgs(kbd, new HwndSource(0, 0, 0, 0, 0, "name", IntPtr.Zero), 0, key);
		}

		/// <summary>
		/// Delta値を指定して<see cref="MouseWheelEventArgs"/>のインスタンスを生成します。
		/// </summary>
		/// <param name="delta">Delta値</param>
		/// <returns><see cref="MouseWheelEventArgs"/>インスタンス</returns>
		private MouseWheelEventArgs GetMouseWheelEventArgs(int delta) {
			var ea = new MouseWheelEventArgs(ObjectCreator.MouseDevice(), 0, delta) {
				RoutedEvent = ObjectCreator.RoutedEvent()
			};
			return ea;
		}

		/// <summary>
		/// Delta値を指定して<see cref="MouseEventArgs"/>のインスタンスを生成します。
		/// </summary>
		/// <returns><see cref="MouseEventArgs"/>インスタンス</returns>
		private MouseEventArgs GetMouseEventArgs() {
			var ea = new MouseEventArgs(ObjectCreator.MouseDevice(), 0) {
				RoutedEvent = ObjectCreator.RoutedEvent()
			};
			return ea;
		}
	}
}
