using NUnit.Framework;

namespace SandBeige.MediaBox.TestUtilities {
	public static class OriginalAssert {
		public static void AreEqual(double? expected, double? actual, double delta) {
			if (expected == null) {
				actual.IsNull();
			} else {
				Assert.AreEqual((double)expected, (double)actual, delta);
			}
		}
	}
}
