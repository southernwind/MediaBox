using System.ComponentModel;
using System.Globalization;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Controls.Converters;

namespace SandBeige.MediaBox.Controls.Tests.Converters {
	[TestFixture]
	internal class ListSortDirectionToBooleanConverterTest {
		[Test]
		public void ConvertWhenTrueIsAscending() {
			var converter = new ListSortDirectionToBooleanConverter {
				ConvertWhenTrue = ListSortDirection.Ascending
			};
			converter.Convert(ListSortDirection.Ascending, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(true);
			converter.Convert(ListSortDirection.Descending, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(false);
			converter.Convert(5, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(false);
			converter.ConvertBack(true, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(ListSortDirection.Ascending);
			converter.ConvertBack(false, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(ListSortDirection.Descending);
			converter.ConvertBack(3, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(ListSortDirection.Descending);
		}

		[Test]
		public void ConvertWhenTrueIsDescending() {
			var converter = new ListSortDirectionToBooleanConverter {
				ConvertWhenTrue = ListSortDirection.Descending
			};
			converter.Convert(ListSortDirection.Ascending, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(false);
			converter.Convert(ListSortDirection.Descending, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(true);
			converter.Convert(5, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(false);
			converter.ConvertBack(true, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(ListSortDirection.Descending);
			converter.ConvertBack(false, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(ListSortDirection.Ascending);
			converter.ConvertBack(3, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(ListSortDirection.Ascending);
		}
	}
}
