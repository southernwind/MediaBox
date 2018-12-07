using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SandBeige.MediaBox.TestUtilities {
	public static class DispatcherUtility {
		public static void DoEvents() {
			var frame = new DispatcherFrame();
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
				new DispatcherOperationCallback(ExitFrame), frame);
			Dispatcher.PushFrame(frame);
		}

		private static object ExitFrame(object frame) {
			((DispatcherFrame)frame).Continue = false;
			return null;
		}
}
}
