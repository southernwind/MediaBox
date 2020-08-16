using System.Globalization;
using System.Windows;

using FluentAssertions;

using Microsoft.Maps.MapControl.WPF;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Controls.Converters;

namespace SandBeige.MediaBox.Controls.Tests.Converters {
	[TestFixture]
	internal class GpsLocationToLocationConverterTest {
		[TestCase(35d, 134d)]
		[TestCase(34.9384983, 135.9120257)]
		[TestCase(20000d, 190d)]
		[TestCase(0d, 0d)]
		[TestCase(-5d, -51d)]
		[TestCase(180d, 180d)]
		public void Convert(double lat, double lon) {
			var converter = new GpsLocationToLocationConverter();
			var location = (Location)converter.Convert(new GpsLocation(lat, lon), typeof(bool), null, CultureInfo.InvariantCulture);
			location.Latitude.Should().Be(lat);
			location.Longitude.Should().Be(lon);
		}

		[TestCase(null)]
		[TestCase(35)]
		[TestCase(35u)]
		public void IsNull(object location) {
			var converter = new GpsLocationToLocationConverter();
			converter.Convert(location, typeof(bool), null, CultureInfo.InvariantCulture).Should().BeNull();
		}

		public void IsUnsetValue() {
			var converter = new GpsLocationToLocationConverter();
			converter.Convert(DependencyProperty.UnsetValue, typeof(bool), null, CultureInfo.InvariantCulture).Should().Be(DependencyProperty.UnsetValue);
		}

		[TestCase(35d, 134d)]
		[TestCase(34.9384983, 135.9120257)]
		[TestCase(20000d, 190d)]
		[TestCase(0d, 0d)]
		[TestCase(-5d, -51d)]
		[TestCase(180d, 180d)]
		public void ConvertBack(double lat, double lon) {
			var converter = new GpsLocationToLocationConverter();
			var location = (GpsLocation)converter.ConvertBack(new Location(lat, lon), typeof(bool), null, CultureInfo.InvariantCulture);
			location.Latitude.Should().Be(lat);
			location.Longitude.Should().Be(lon);
		}
	}
}
