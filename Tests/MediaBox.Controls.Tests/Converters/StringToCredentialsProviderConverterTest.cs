using System;
using System.Globalization;
using Microsoft.Maps.MapControl.WPF.Core;
using NUnit.Framework;
using SandBeige.MediaBox.Controls.Converters;

namespace SandBeige.MediaBox.Controls.Tests.Converters {
	[TestFixture]
	internal class StringToCredentialsProviderConverterTest {
		[TestCase("")]
		[TestCase("4DYT(dYznRZhRu7~6_9hMG8ktfErR,U$xRNJ~J7t")]
		[TestCase("KEy!ZSbtE-946#Hye*_7yv4Q4MrG~V$AZuqLGfD&9EDv+zMSYdMR)rgSNyLa(vf~yWW+yr5XMf%f6n.e")]
		public void Convert(object obj) {
			var converter = new StringToCredentialsProviderConverter();
			var cp = (CredentialsProvider)converter.Convert(obj, typeof(bool), null, CultureInfo.InvariantCulture);
			string credential = null;
			cp.GetCredentials(x => {
				credential = x.ApplicationId;
			});
			Assert.AreEqual(obj, credential);
		}

		[TestCase(null)]
		[TestCase(5)]
		[TestCase(true)]
		[TestCase('f')]
		public void ConvertInvalidValue(object obj) {
			var converter = new StringToCredentialsProviderConverter();
			var cp = (CredentialsProvider)converter.Convert(obj, typeof(bool), null, CultureInfo.InvariantCulture);
			string credential = null;
			cp.GetCredentials(x => {
				credential = x.ApplicationId;
			});
			Assert.AreEqual("", credential);
		}

		[TestCase("aa")]
		public void ConvertBack(string value) {
			var converter = new StringToCredentialsProviderConverter();
			Assert.Throws<NotImplementedException>(() => {
				converter.ConvertBack(new CredentialsProviderImp(value), typeof(bool), null, CultureInfo.InvariantCulture);
			});
		}

		[TestCase(1)]
		[TestCase(0)]
		[TestCase(null)]
		[TestCase("aa")]
		[TestCase(true)]
		public void ConvertBackInvalidValue(object obj) {
			var converter = new StringToCredentialsProviderConverter();
			Assert.Throws<NotImplementedException>(() => {
				converter.ConvertBack(obj, typeof(bool), null, CultureInfo.InvariantCulture);
			});
		}
	}
}
