using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;
using NUnit.Framework;

namespace SandBeige.MediaBox.Tests.Views {
	internal class ViewTestClassBase {
		[OneTimeSetUp]
		public void OneTimeSetUp() {
			if (Application.Current == null) {
				new App();
			}
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\MediaBox\Views\Converters.xaml");

			Application.Current.Resources = (ResourceDictionary)XamlServices.Load(path);
		}

	}
}
