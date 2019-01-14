using System.ComponentModel;
using System.Globalization;

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
			converter.Convert(ListSortDirection.Ascending, typeof(bool), null, CultureInfo.InvariantCulture).Is(true);
			converter.Convert(ListSortDirection.Descending, typeof(bool), null, CultureInfo.InvariantCulture).Is(false);
			converter.Convert(5, typeof(bool), null, CultureInfo.InvariantCulture).Is(false);
			converter.ConvertBack(true, typeof(bool), null, CultureInfo.InvariantCulture).Is(ListSortDirection.Ascending);
			converter.ConvertBack(false, typeof(bool), null, CultureInfo.InvariantCulture).Is(ListSortDirection.Descending);
			converter.ConvertBack(3, typeof(bool), null, CultureInfo.InvariantCulture).Is(ListSortDirection.Descending);
		}

		[Test]
		public void ConvertWhenTrueIsDescending() {
			var converter = new ListSortDirectionToBooleanConverter {
				ConvertWhenTrue = ListSortDirection.Descending
			};
			converter.Convert(ListSortDirection.Ascending, typeof(bool), null, CultureInfo.InvariantCulture).Is(false);
			converter.Convert(ListSortDirection.Descending, typeof(bool), null, CultureInfo.InvariantCulture).Is(true);
			converter.Convert(5, typeof(bool), null, CultureInfo.InvariantCulture).Is(false);
			converter.ConvertBack(true, typeof(bool), null, CultureInfo.InvariantCulture).Is(ListSortDirection.Descending);
			converter.ConvertBack(false, typeof(bool), null, CultureInfo.InvariantCulture).Is(ListSortDirection.Ascending);
			converter.ConvertBack(3, typeof(bool), null, CultureInfo.InvariantCulture).Is(ListSortDirection.Ascending);
		}
	}
}
