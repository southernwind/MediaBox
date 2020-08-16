using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Map;

namespace SandBeige.MediaBox.Library.Tests.Map {
	[TestFixture]
	internal class GeoCodingTest {
		[Test]
		public async Task パターン1() {
			var geo = new GeoCoding();
			var _ = await geo.Reverse(34.22795833, 131.303619444);
		}


		[Test]
		public async Task パターン2() {
			var geo = new GeoCoding();
			var _ = await geo.Reverse(35.628852, 139.882162);
		}
	}
}
