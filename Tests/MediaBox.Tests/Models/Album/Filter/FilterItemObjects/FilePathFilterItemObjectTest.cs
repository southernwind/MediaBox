using System;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemObjects {
	internal class FilePathFilterItemCreatorTest : ModelTestClassBase {
		[TestCase("image", SearchTypeInclude.Include, "imageをファイルパスに含む")]
		[TestCase("raccoon", SearchTypeInclude.Exclude, "raccoonをファイルパスに含まない")]
		public void プロパティ(string text, SearchTypeInclude searchType, string displayName) {
			var io = new FilePathFilterItemObject(text, searchType);
			io.Text.Should().Be(text);
			io.SearchType.Should().Be(searchType);
			io.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var io2 = new FilePathFilterItemObject();
#pragma warning restore 618
			io2.Text.Should().BeNull();
			io2.SearchType.Should().Be(SearchTypeInclude.Include);
			io2.Text = text;
			io2.SearchType = searchType;
			io2.Text.Should().Be(text);
			io2.SearchType.Should().Be(searchType);
			io2.DisplayName.Should().Be(displayName);
		}

		[TestCase(null, SearchTypeInclude.Include)]
		[TestCase("file", (SearchTypeInclude)100)]
		public void フィルター作成例外(string text, SearchTypeInclude searchType) {
			Action act = () => {
				_ = new FilePathFilterItemObject(text, searchType);
			};
			act.Should().ThrowExactly<ArgumentException>();
		}
	}
}
