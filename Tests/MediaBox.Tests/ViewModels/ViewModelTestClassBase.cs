using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.ViewModels {
	internal class ViewModelTestClassBase : TestClassBase {

		[OneTimeSetUp]
		public override void OneTimeSetUp() {
			base.OneTimeSetUp();
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
		}

		[SetUp]
		public override void SetUp() {
			base.SetUp();
		}
	}
}
