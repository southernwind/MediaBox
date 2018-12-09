using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.ViewModels.ValidationAttributes;

namespace SandBeige.MediaBox.Tests.ViewModels.ValidationAttributes {
	[TestFixture]
	internal class ExistsDirectoryAttributeTest {
		[Test]
		public void Test() {
			_ = new ExistsDirectoryAttribute();
		}
	}
}
