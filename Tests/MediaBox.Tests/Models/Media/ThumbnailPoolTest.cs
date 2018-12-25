using System.IO;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class ThumbnailPoolTest : TestClassBase {
		[Test]
		public void RegisterResolve() {
			var pool = Get.Instance<ThumbnailPool>();
			var path1 = Path.Combine(TestDirectories["0"], "image1.jpg");
			var path2 = Path.Combine(TestDirectories["0"], "image2.jpg");
			var path3 = Path.Combine(TestDirectories["0"], "image3.jpg");

			// 登録されていなければnull
			pool.Resolve(path1).IsNull();

			// 登録
			pool.Register(path1, File.ReadAllBytes(path1));
			pool.Register(path2, File.ReadAllBytes(path2));

			// 登録されていればその値
			pool.Resolve(path1).Is(File.ReadAllBytes(path1));

			// 登録されていなければ登録した上で値取得
			pool.Resolve(path3).IsNull();

			pool.ResolveOrRegister(path3, () => File.ReadAllBytes(path3)).Is(File.ReadAllBytes(path3));
			// 以後は取得可能に
			pool.Resolve(path3).Is(File.ReadAllBytes(path3));
		}
	}
}
