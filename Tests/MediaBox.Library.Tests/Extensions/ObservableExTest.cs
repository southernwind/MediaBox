using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using NUnit.Framework;
using SandBeige.MediaBox.Library.Extensions;
using static SandBeige.MediaBox.Library.Extensions.ObservableEx;

namespace SandBeige.MediaBox.Library.Tests.Extensions {
	[TestFixture]
	internal class ObservableExTest {
		[Test]
		public void ToOldAndNewValue() {
			var subject = new Subject<int>();
			var list = new List<OldAndNewValue<int>>();
			subject.ToOldAndNewValue().Subscribe(x=> {
				list.Add(x);
			});
			subject.OnNext(5);
			Assert.AreEqual(0, list.Count);

			subject.OnNext(15);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(5, list[0].OldValue);
			Assert.AreEqual(15, list[0].NewValue);

			subject.OnNext(40);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(15, list[1].OldValue);
			Assert.AreEqual(40, list[1].NewValue);
		}
	}
}
