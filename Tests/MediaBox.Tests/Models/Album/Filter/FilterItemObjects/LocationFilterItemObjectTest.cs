using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemObjects {
	internal class LocationFilterItemCreatorTest : ModelTestClassBase {
		[TestCase(true, "座標情報を含む")]
		[TestCase(false, "座標情報を含まない")]
		public void プロパティ(bool contains, string displayName) {
			var io = new LocationFilterItemObject(contains);
			io.Contains.Should().Be(contains);
			io.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var io2 = new LocationFilterItemObject();
#pragma warning restore 618
			io2.Contains.Should().BeNull();
			io2.Contains = contains;
			io2.Contains.Should().Be(contains);
			io2.DisplayName.Should().Be(displayName);
		}
	}
}
