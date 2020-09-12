using System;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemObjects {
	internal class TagFilterItemObjectTest : ModelTestClassBase {
		[TestCase("tag", SearchTypeInclude.Include, "tagをタグに含む")]
		[TestCase("raccoon", SearchTypeInclude.Exclude, "raccoonをタグに含まない")]
		public void プロパティ(string tag, SearchTypeInclude searchType, string displayName) {
			var io = new TagFilterItemObject(tag, searchType);
			io.TagName.Should().Be(tag);
			io.SearchType.Should().Be(searchType);
			io.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var io2 = new TagFilterItemObject();
#pragma warning restore 618
			io2.TagName.Should().BeNull();
			io2.SearchType.Should().Be(SearchTypeInclude.Include);
			io2.TagName = tag;
			io2.SearchType = searchType;
			io2.TagName.Should().Be(tag);
			io2.SearchType.Should().Be(searchType);
			io2.DisplayName.Should().Be(displayName);
		}

		[TestCase(null, SearchTypeInclude.Include)]
		[TestCase("tag", (SearchTypeInclude)100)]
		public void フィルター作成例外(string tag, SearchTypeInclude searchType) {
			Action act = () => {
				_ = new TagFilterItemObject(tag, searchType);
			};
			act.Should().ThrowExactly<ArgumentException>();
		}
	}
}
