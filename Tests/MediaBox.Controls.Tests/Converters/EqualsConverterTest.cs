using System;
using System.Globalization;

using NUnit.Framework;

using SandBeige.MediaBox.Controls.Converters;

namespace SandBeige.MediaBox.Controls.Tests.Converters {
	[TestFixture]
	internal class EqualsConverterTest {
		[TestCase(false, 1, 2)]
		[TestCase(false, 1, 1d)]
		[TestCase(false, 1, true)]
		[TestCase(false, 0, true)]
		[TestCase(false, 0, false)]
		[TestCase(false, 1, false)]
		[TestCase(false, -1, false)]
		[TestCase(false, "1", 1)]
		[TestCase(true, 1, 1)]
		[TestCase(true, false, false)]
		[TestCase(false, null, 1)]
		[TestCase(true, null, null)]
		[TestCase(false, "aaa", "bbb")]
		[TestCase(true, "aaa", "aaa")]
		[TestCase(true, "", "")]
		public void Convert(bool result, object object1, object object2) {
			var converter = new EqualsConverter();
			converter.Convert(new[] { object1, object2 }, typeof(bool), null, CultureInfo.InvariantCulture).Is(result);
			converter.Convert(new[] { object2, object1 }, typeof(bool), null, CultureInfo.InvariantCulture).Is(result);
		}

		[Test]
		public void Convert2() {
			var object1 = new object();
			var object2 = object1;
			this.Convert(true, object1, object2);
			object2 = new object();
			this.Convert(false, object1, object2);
			this.Convert(true, (1, 2, "aa"), (1, 2, "aa"));
			this.Convert(true, new {
				a = 1,
				b = 2,
				c = "aa"
			}, new {
				a = 1,
				b = 2,
				c = "aa"
			});
		}

		[TestCase(1)]
		[TestCase(0)]
		[TestCase(null)]
		[TestCase("aa")]
		public void ConvertBack(object value) {
			var converter = new EqualsConverter();
			Assert.Throws<NotSupportedException>(() => {
				converter.ConvertBack(value, new[] { typeof(bool) }, null, CultureInfo.InvariantCulture);
			});
		}
	}
}
