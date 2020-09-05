using System;
using System.IO;

using NUnit.Framework;

using SandBeige.MediaBox.TestUtilities.TestData;

namespace SandBeige.MediaBox.TestUtilities {
	[TestFixture]
	public class TestClassBase {
		protected string TestDataDir = null!;
		protected TestFiles TestFiles = null!;

		[OneTimeSetUp]
		public virtual void OneTimeSetUp() {
			this.TestDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, @"TestData\File");
			this.TestFiles = new TestFiles(this.TestDataDir);
		}

		[SetUp]
		public virtual void SetUp() {
		}

		[TearDown]
		public virtual void TearDown() {
		}
	}
}
