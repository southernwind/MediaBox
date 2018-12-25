using System;
using System.Collections.Generic;
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
			subject.ToOldAndNewValue().Subscribe(x => {
				list.Add(x);
			});
			subject.OnNext(5);
			list.Count.Is(0);

			subject.OnNext(15);
			list.Count.Is(1);
			list[0].OldValue.Is(5);
			list[0].NewValue.Is(15);

			subject.OnNext(40);
			list.Count.Is(2);
			list[1].OldValue.Is(15);
			list[1].NewValue.Is(40);
		}
	}
}
