using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class ThumbnailPoolTest :TestClassBase {
		[Test]
		public void RegisterResolve() {
			var pool = Get.Instance<ThumbnailPool>();
			var path1 = Path.Combine(TestDirectories["0"], "image1.jpg");
			var path2 = Path.Combine(TestDirectories["0"], "image2.jpg");
			var path3 = Path.Combine(TestDirectories["0"], "image3.jpg");

			// 登録されていなければnull
			Assert.IsNull(pool.Resolve(path1));

			// 登録
			pool.Register(path1, File.ReadAllBytes(path1));
			pool.Register(path2, File.ReadAllBytes(path2));

			// 登録されていればその値
			CollectionAssert.AreEqual(File.ReadAllBytes(path1), pool.Resolve(path1));

			// 登録されていなければ登録した上で値取得
			Assert.IsNull(pool.Resolve(path3));
			Assert.AreEqual(
				File.ReadAllBytes(path3),
				pool.ResolveOrRegister(path3, () => File.ReadAllBytes(path3)));
			// 以後は取得可能に
			Assert.AreEqual(
				File.ReadAllBytes(path3),
				pool.Resolve(path3));
		}
	}
}
