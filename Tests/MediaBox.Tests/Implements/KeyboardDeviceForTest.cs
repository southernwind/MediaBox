using System.Linq;
using System.Windows.Input;

namespace SandBeige.MediaBox.Tests.Implements {
	internal class KeyboardDeviceForTest : KeyboardDevice {

		public Key[] DownKeys {
			get;
			set;
		} = { };

		public Key[] ToggledKeys {
			get;
			set;
		} = { };
		public KeyboardDeviceForTest(InputManager inputManager) : base(inputManager) {
		}

		protected override KeyStates GetKeyStatesFromSystem(Key key) {
			if (this.DownKeys.Contains(key)) {
				return KeyStates.Down;
			}

			if (this.ToggledKeys.Contains(key)) {
				return KeyStates.Toggled;
			}

			return KeyStates.None;
		}
	}
}
