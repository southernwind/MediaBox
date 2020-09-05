using FluentAssertions;

namespace SandBeige.MediaBox.TestUtilities {
	public static class OriginalAssert {
		public static void AreEqual(double? expected, double? actual, double delta) {
			if (expected == null) {
				actual.Should().BeNull();
			} else {
				actual.Should().BeApproximately(expected, delta);
			}
		}
	}
}
