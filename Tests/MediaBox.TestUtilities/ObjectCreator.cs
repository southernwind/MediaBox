using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace SandBeige.MediaBox.TestUtilities {
	public static class ObjectCreator {
		public static MouseDevice MouseDevice() {
			var type = Assembly
				.GetAssembly(typeof(MouseDevice))
				.GetType("System.Windows.Input.Win32MouseDevice");

			return (MouseDevice)type
				.Assembly
				.CreateInstance(
					type.FullName,
					false,
					BindingFlags.NonPublic | BindingFlags.Instance,
					null,
					new object[] { InputManager.Current },
					null,
					null);
		}

		public static RoutedEvent RoutedEvent() {
			var type = typeof(RoutedEvent);

			return (RoutedEvent)type
				.Assembly
				.CreateInstance(
					type.FullName,
					false,
					BindingFlags.NonPublic | BindingFlags.Instance,
					null,
					new object[] { "name", RoutingStrategy.Direct, typeof(RoutedEventHandler), null },
					null,
					null);
		}
	}
}
