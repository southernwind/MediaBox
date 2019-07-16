using System.Globalization;
using System.Windows;

using NUnit.Framework;

using SandBeige.MediaBox.Controls.Converters;

namespace SandBeige.MediaBox.Controls.Tests.Converters {
	[TestFixture]
	internal class InvertBooleanToVisibilityConverterTest {
		[TestCase(false, Visibility.Visible)]
		[TestCase(true, Visibility.Collapsed)]
		[TestCase(5, Visibility.Visible)]
		public void Convert(object boolean, Visibility visibility) {
			var converter = new InvertBooleanToVisibilityConverter();
			converter.Convert(boolean, typeof(bool), null, CultureInfo.InvariantCulture).Is(visibility);
		}

		[TestCase(Visibility.Visible, false)]
		[TestCase(Visibility.Collapsed, true)]
		[TestCase(Visibility.Hidden, false)]
		[TestCase(5, false)]
		public void ConvertBack(object visibility, bool boolean) {
			var converter = new InvertBooleanToVisibilityConverter();
			converter.ConvertBack(visibility, typeof(bool), null, CultureInfo.InvariantCulture).Is(boolean);
		}
	}
}
