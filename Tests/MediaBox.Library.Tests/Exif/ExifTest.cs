using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Collection;

namespace SandBeige.MediaBox.Library.Tests.Exif {
	[TestFixture]
	internal class ExifTest {
		private static string _testDataDir;

		[SetUp]
		public void SetUp() {
			_testDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\");
		}

		[TestCase(null, "image1.heic")]
		[TestCase(null, "image2.png")]
		[TestCase("iPhone 6s", "image3.jpg")]
		[TestCase(null, "image4.jpg")]
		public void TypeVariations(string result, string fileName) {
			var exif = new Library.Exif.Exif(Path.Combine(_testDataDir, fileName));
			exif.Model.Is(result);
		}

		[Test]
		public void ThrowException() {
			Assert.Catch<FileNotFoundException>(() => {
				_ = new Library.Exif.Exif(Path.Combine(_testDataDir, "image.jpg"));
			});
		}

		[Test]
		public void ToTitleValuePair() {
			var exif = new Library.Exif.Exif(Path.Combine(_testDataDir, "image4.jpg"));
			exif.ToTitleValuePair().Count().Is(0);
			exif = new Library.Exif.Exif(Path.Combine(_testDataDir, "image3.jpg"));
			var tvp = exif.ToTitleValuePair();
			tvp.ToDictionary(x => x.Title, x => x.Value).Is(
				new[] {
				new TitleValuePair<string>("メーカー","Apple"),
				new TitleValuePair<string>("モデル","iPhone 6s"),
				new TitleValuePair<string>("画像の方向","1"),
				new TitleValuePair<string>("サイズ","72 × 72 inches"),
				new TitleValuePair<string>("ファイル変更日時","2018:07:02 14:32:01"),
				new TitleValuePair<string>("露出時間","0.025"),
				new TitleValuePair<string>("色空間情報","1"),
				new TitleValuePair<string>("露出モード","0"),
				new TitleValuePair<string>("ホワイトバランス","0"),
				new TitleValuePair<string>("緯度","北緯35度42分36.26秒"),
				new TitleValuePair<string>("経度","東経139度48分34.07秒"),
				new TitleValuePair<string>("高度","17.4395658608781")
			}.ToDictionary(x => x.Title, x => x.Value));
		}
	}
}
