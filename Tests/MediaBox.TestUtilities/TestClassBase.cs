using System;
using System.IO;

using NUnit.Framework;

using SandBeige.MediaBox.TestUtilities.TestData;

namespace SandBeige.MediaBox.TestUtilities {
	[TestFixture]
	public class TestClassBase {
		protected string TestDataDir;
		protected TestFiles TestFiles;

		[OneTimeSetUp]
		public virtual void OneTimeSetUp() {
		}

		[SetUp]
		public virtual void SetUp() {
			this.TestDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\File");
			this.TestFiles = new TestFiles(this.TestDataDir);
		}

		[TearDown]
		public virtual void TearDown() {
		}
	}
}
