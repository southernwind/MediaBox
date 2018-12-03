using System;
using System.Globalization;
using System.Windows;
using NUnit.Framework;
using SandBeige.MediaBox.Controls.Converters;

namespace SandBeige.MediaBox.Controls.Tests.Converters {
	[TestFixture]
	internal class AllValuesAreSetConverterTest {
		[TestCase(false, new object[] { 1, 2, 3, "DependencyProperty.UnsetValue" })]
		[TestCase(false, new object[] { "DependencyProperty.UnsetValue" })]
		[TestCase(false, new object[] { null })]
		[TestCase(false, new object[] { "DependencyProperty.UnsetValue", 1, 2, 3 })]
		[TestCase(false, new object[] { null, 1, 2, 3 })]
		[TestCase(false, new object[] { "a", 'c', true, null })]
		[TestCase(false, new object[] { "a", 'c', true, "DependencyProperty.UnsetValue" })]
		[TestCase(false, new object[] { "a", 'c', null, "DependencyProperty.UnsetValue" })]
		[TestCase(true, new object[] { 1, 2, 3 })]
		[TestCase(true, new object[] { "a", 'c', true })]
		[TestCase(true, new object[] { 0 })]
		[TestCase(true, new object[] { "" })]
		[TestCase(true, new object[] { })]
		public void Convert(bool result, object[] values) {
			for (var i = 0; i < values.Length; i++) {
				// 属性引数に定数しか入れられないので、苦肉の策
				if (values[i] is string str && str == "DependencyProperty.UnsetValue") {
					values[i] = DependencyProperty.UnsetValue;
				}
			}
			var converter = new AllValuesAreSetConverter();
			Assert.AreEqual(result, converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture));
		}

		[TestCase(1)]
		[TestCase(0)]
		[TestCase(null)]
		[TestCase("aa")]
		public void ConvertBack(object value) {
			var converter = new AllValuesAreSetConverter();
			Assert.Throws<NotImplementedException>(() => {
				converter.ConvertBack(value, new[] { typeof(bool) }, null, CultureInfo.InvariantCulture);
			});
		}
	}
}
