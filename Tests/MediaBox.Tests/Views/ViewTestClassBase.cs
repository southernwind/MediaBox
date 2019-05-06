using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Xaml;

using NUnit.Framework;

namespace SandBeige.MediaBox.Tests.Views {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal abstract class ViewTestClassBase<T> where T : new() {
		[OneTimeSetUp]
		public void OneTimeSetUp() {
			if (Application.Current == null) {
				_ = new App();
			}
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\MediaBox\Views\Resources\Resources.xaml");

			Application.Current.Resources = (ResourceDictionary)XamlServices.Load(path);
		}

		[Test]
		public void インスタンス生成() {
			_ = new T();
		}
	}
}
