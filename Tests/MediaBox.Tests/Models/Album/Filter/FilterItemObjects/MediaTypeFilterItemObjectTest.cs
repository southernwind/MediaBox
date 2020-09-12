using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemObjects {
	internal class MediaTypeFilterItemCreatorTest : ModelTestClassBase {
		[TestCase(true, "動画ファイル")]
		[TestCase(false, "画像ファイル")]
		public void プロパティ(bool isVideo, string displayName) {
			var io = new MediaTypeFilterItemObject(isVideo);
			io.IsVideo.Should().Be(isVideo);
			io.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var io2 = new MediaTypeFilterItemObject();
#pragma warning restore 618
			io2.IsVideo.Should().Be(false);
			io2.IsVideo = isVideo;
			io2.IsVideo.Should().Be(isVideo);
			io2.DisplayName.Should().Be(displayName);
		}
	}
}
