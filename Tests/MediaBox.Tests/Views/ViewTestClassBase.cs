using System;
using System.IO;
using System.Windows;
using System.Xaml;

using NUnit.Framework;

namespace SandBeige.MediaBox.Tests.Views {
	internal class ViewTestClassBase {
		[OneTimeSetUp]
		public void OneTimeSetUp() {
			if (Application.Current == null) {
				_ = new App();
			}
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\MediaBox\Views\Resources\Resources.xaml");

			Application.Current.Resources = (ResourceDictionary)XamlServices.Load(path);
		}

	}
}
