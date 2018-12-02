using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Library.Tests.Exif {
	[TestFixture]
	internal class ExifTest {
		private static string testdataDir;

		[SetUp]
		public void SetUp() {
			testdataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Exif\TestData\");
		}

		[TestCase(null, "image1.heic")]
		[TestCase(null ,"image2.png")]
		[TestCase("iPhone 6s", "image3.jpg")]
		[TestCase(null, "image4.jpg")]
		public void TypeValiations(string result,string fileName) {
			var exif = new Models.Media.Exif(Path.Combine(testdataDir, fileName));
			Assert.AreEqual(result, exif.Model);
		}

		[Test]
		public void ThrowException() {
			Assert.Catch<FileNotFoundException>(()=> {
				var exif = new Models.Media.Exif(Path.Combine(testdataDir, "image.jpg"));
			});
		}

		[Test]
		public void ToTitleValuePair() {
			var exif = new Models.Media.Exif(Path.Combine(testdataDir, "image4.jpg"));
			Assert.AreEqual(0, exif.ToTitleValuePair().Count());
			exif = new Models.Media.Exif(Path.Combine(testdataDir, "image3.jpg"));
			var tvp = exif.ToTitleValuePair();
			CollectionAssert.AreEqual(new[] {
				new TitleValuePair("メーカー","Apple"),
				new TitleValuePair("モデル","iPhone 6s"),
				new TitleValuePair("画像の方向","1"),
				new TitleValuePair("サイズ","72 × 72 inches"),
				new TitleValuePair("ファイル変更日時","2018:07:02 14:32:01"),
				new TitleValuePair("露出時間","0.025"),
				new TitleValuePair("色空間情報","1"),
				new TitleValuePair("露出モード","0"),
				new TitleValuePair("ホワイトバランス","0"),
				new TitleValuePair("緯度","北緯35度42分36.26秒"),
				new TitleValuePair("経度","東経139度48分34.07秒"),
				new TitleValuePair("高度","17.4395658608781")
			}.ToDictionary(x => x.Title, x => x.Value)
			, tvp.ToDictionary(x => x.Title, x => x.Value));
		}
	}
}
