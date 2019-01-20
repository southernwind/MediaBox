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
			var path3 = Path.Combine(TestDataDir, "image3.jpg");

			// 1回目で登録される
			var th1 = pool.ResolveOrRegisterByFullSizeFilePath(path1, ThumbnailLocation.Memory);
			th1.Location.Is(ThumbnailLocation.Memory);
			// 2回目以降は同じインスタンスが取得できる
			pool.ResolveOrRegisterByFullSizeFilePath(path1, ThumbnailLocation.Memory).Is(th1);
			th1.Location.Is(ThumbnailLocation.Memory);
			// 生成先に変更があった場合、新しい生成先にも作られる
			pool.ResolveOrRegisterByFullSizeFilePath(path1, ThumbnailLocation.File).Is(th1);
			th1.Location.Is(ThumbnailLocation.File | ThumbnailLocation.Memory);

			// キーが変わると新しいインスタンスになる
			var th2 = pool.ResolveOrRegisterByFullSizeFilePath(path2, ThumbnailLocation.File);
			th2.IsNot(th1);
			th2.Location.Is(ThumbnailLocation.File);
			pool.ResolveOrRegisterByFullSizeFilePath(path2, ThumbnailLocation.Memory).Is(th2);
			th2.Location.Is(ThumbnailLocation.File | ThumbnailLocation.Memory);

			// サムネイルファイル名から作成
			var th3 = pool.ResolveOrRegisterByThumbnailFileName(path3, th1.FileName);
			th3.IsNot(th2);
			th3.IsNot(th1);
			th3.FileName.Is(th1.FileName);
			th3.Location.Is(ThumbnailLocation.File);

			// 生成方法が違ってもキーが同じなら同じインスタンス
			pool.ResolveOrRegisterByThumbnailFileName(path1, "").Is(th1);
			pool.ResolveOrRegisterByFullSizeFilePath(path3, ThumbnailLocation.Memory).Is(th3);
			th3.Location.Is(ThumbnailLocation.File | ThumbnailLocation.Memory);

		}
	}
}
