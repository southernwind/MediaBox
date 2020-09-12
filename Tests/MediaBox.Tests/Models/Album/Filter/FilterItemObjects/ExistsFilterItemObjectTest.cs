using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemObjects {
	internal class ExistsItemCreatorTest : ModelTestClassBase {
		[TestCase(true, "ファイルが存在する")]
		[TestCase(false, "ファイルが存在しない")]
		public void プロパティ(bool contains, string displayName) {
			var io = new ExistsFilterItemObject(contains);
			io.Exists.Should().Be(contains);
			io.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var io2 = new ExistsFilterItemObject();
#pragma warning restore 618
			io2.Exists.Should().Be(false);
			io2.Exists = contains;
			io2.Exists.Should().Be(contains);
			io2.DisplayName.Should().Be(displayName);
		}
	}
}
