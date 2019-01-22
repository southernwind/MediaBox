using System.IO;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class ThumbnailPoolTest : TestClassBase {
		[Test]
		public void ResolveOrRegister() {
			var pool = Get.Instance<ThumbnailPool>();
			var path1 = Path.Combine(TestDataDir, "image1.jpg");
			var path2 = Path.Combine(TestDataDir, "image2.jpg");

			// 1回目で登録される
			var th1 = pool.ResolveOrRegister(path1);
			th1.Enabled.IsFalse();
			// 2回目以降は同じインスタンスが取得できる
			pool.ResolveOrRegister(path1).Is(th1);

			// キーが変わると新しいインスタンスになる
			pool.ResolveOrRegister(path2).IsNot(th1);
		}
	}
}
