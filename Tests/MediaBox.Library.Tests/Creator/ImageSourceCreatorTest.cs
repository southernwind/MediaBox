using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Creator;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Library.Tests.Creator {
	[TestFixture]
	public class ImageSourceCreatorTest : TestClassBase {

		[TestCase(Rotation.Rotate0, false, null)]
		[TestCase(Rotation.Rotate0, false, 0)]
		[TestCase(Rotation.Rotate0, false, 1)]
		[TestCase(Rotation.Rotate0, true, 2)]
		[TestCase(Rotation.Rotate180, false, 3)]
		[TestCase(Rotation.Rotate180, true, 4)]
		[TestCase(Rotation.Rotate270, true, 5)]
		[TestCase(Rotation.Rotate90, false, 6)]
		[TestCase(Rotation.Rotate90, true, 7)]
		[TestCase(Rotation.Rotate270, false, 8)]
		public async Task RotationTransform(Rotation rotation, bool isFlipped, int? orientation) {
			var image = ImageSourceCreator.Create(this.TestFiles.Image1Jpg.FilePath, orientation);
			var image2 = await ImageSourceCreator.CreateAsync(this.TestFiles.Image1Jpg.FilePath, orientation)!;
			if (isFlipped) {
				var tb = (TransformedBitmap)image;
				var tb2 = (TransformedBitmap)image2!;
				tb.Should().NotBeNull();
				tb2.Should().NotBeNull();
				tb.Transform.Value.M11.Should().Be(-1);
				tb2.Transform.Value.M11.Should().Be(-1);
				tb.Transform.Value.M22.Should().Be(1);
				tb2.Transform.Value.M22.Should().Be(1);
				((BitmapImage)tb.Source).Rotation.Should().Be(rotation);
				((BitmapImage)tb2.Source).Rotation.Should().Be(rotation);
			} else {
				var bi = (BitmapImage)image;
				var bi2 = (BitmapImage)image2!;
				bi.Should().NotBeNull();
				bi2.Should().NotBeNull();
				bi.Rotation.Should().Be(rotation);
				bi2.Rotation.Should().Be(rotation);
			}

			image.IsFrozen.Should().BeTrue();
			image2!.IsFrozen.Should().BeTrue();
		}

		[TestCase(7, 5, 0, 0)]
		[TestCase(3, 2, 3, 2)]
		[TestCase(7, 5, 7, 5)]
		[TestCase(4, 4, 4, 4)]
		public async Task PixelWidthHeight(int resultWidth, int resultHeight, double limitWidth, double limitHeight) {
			var image = (BitmapImage)ImageSourceCreator.Create(this.TestFiles.Image1Jpg.FilePath, 1, limitWidth, limitHeight);
			var image2 = (BitmapImage?)await ImageSourceCreator.CreateAsync(this.TestFiles.Image1Jpg.FilePath, 1, limitWidth, limitHeight);
			image.PixelWidth.Should().Be(resultWidth);
			image2!.PixelWidth.Should().Be(resultWidth);
			image.PixelHeight.Should().Be(resultHeight);
			image2.PixelHeight.Should().Be(resultHeight);
		}

		[Test]
		public async Task Source() {
			var image = ImageSourceCreator.Create(this.TestFiles.Image1Jpg.FilePath);
			var image2 = await ImageSourceCreator.CreateAsync(this.TestFiles.Image1Jpg.FilePath);
			image.Should().NotBeNull();
			image2.Should().NotBeNull();
			var stream = new FileStream(this.TestFiles.Image1Jpg.FilePath, FileMode.Open, FileAccess.Read);
			image = ImageSourceCreator.Create(stream);
			image2 = await ImageSourceCreator.CreateAsync(this.TestFiles.Image1Jpg.FilePath);
			image.Should().NotBeNull();
			image2.Should().NotBeNull();

			Assert.Catch<ArgumentException>(() => {
				ImageSourceCreator.Create(5);
			});
		}

		[Test]
		public async Task CancellationToken() {
			var token = new CancellationTokenSource();
			var token2 = new CancellationTokenSource();
			var task = ImageSourceCreator.CreateAsync(this.TestFiles.Image1Jpg.FilePath, token: token.Token);
			var task2 = ImageSourceCreator.CreateAsync(this.TestFiles.Image1Jpg.FilePath, token: token2.Token);
			token.Cancel();
			(await task).Should().BeNull();
			(await task2).Should().NotBeNull();
		}
	}
}
