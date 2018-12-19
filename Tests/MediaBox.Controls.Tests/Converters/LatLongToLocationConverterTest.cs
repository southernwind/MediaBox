using System.Globalization;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using NUnit.Framework;
using SandBeige.MediaBox.Controls.Converters;

namespace SandBeige.MediaBox.Controls.Tests.Converters {
	[TestFixture]
	internal class LatLongToLocationConverterTest {
		[TestCase(35d, 134d)]
		[TestCase(34.9384983, 135.9120257)]
		[TestCase(20000d, 190d)]
		[TestCase(0d, 0d)]
		[TestCase(-5d, -51d)]
		[TestCase(180d, 180d)]
		public void Convert(object lat, object lon) {
			var converter = new LatLongToLocationConverter();
			var location = (Location)converter.Convert(new[] { lat, lon }, typeof(bool), null, CultureInfo.InvariantCulture);
			Assert.AreEqual(lat, location.Latitude);
			Assert.AreEqual(lon, location.Longitude);
		}

		[TestCase(null, 134d)]
		[TestCase(35d, null)]
		[TestCase(35d, 1)]
		[TestCase(5, 134d)]
		[TestCase(null, null)]
		[TestCase(5, 134)]
		[TestCase(5u, 134u)]
		public void IsNull(object lat, object lon) {
			var converter = new LatLongToLocationConverter();
			Assert.IsNull(converter.Convert(new[] { lat, lon }, typeof(bool), null, CultureInfo.InvariantCulture));
		}

		[TestCase(null, 134d)]
		[TestCase(35d, null)]
		public void IsUnsetValue(object lat, object lon) {
			var converter = new LatLongToLocationConverter();
			Assert.AreEqual(
				DependencyProperty.UnsetValue,
				converter.Convert(new[] { lat ?? DependencyProperty.UnsetValue, lon ?? DependencyProperty.UnsetValue }, typeof(bool), null, CultureInfo.InvariantCulture));
		}

		[TestCase(35d, 134d)]
		[TestCase(34.9384983, 135.9120257)]
		[TestCase(20000d, 190d)]
		[TestCase(0d, 0d)]
		[TestCase(-5d, -51d)]
		[TestCase(180d, 180d)]
		public void ConvertBack(double lat, double lon) {
			var converter = new LatLongToLocationConverter();
			CollectionAssert.AreEqual(
				new[] { lat, lon },
				converter.ConvertBack(new Location(lat, lon), new[] { typeof(bool) }, null, CultureInfo.InvariantCulture)
			);
		}
	}
}
