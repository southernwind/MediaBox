using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Input;

using FluentAssertions;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.God;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.God {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class ToZoomLevelTest {
		[Test]
		public void 初期値なし() {
			var source = new Subject<MouseWheelEventArgs>();
			var level = source.ToZoomLevel();
			level.Value.Should().Be(1);
			source.OnNext(this.GetMouseWheelEventArgs(10));
			level.Value.Should().Be(2);
		}

		[Test]
		public void 初期値あり() {
			var source = new Subject<MouseWheelEventArgs>();
			var level = source.ToZoomLevel(new ReactivePropertySlim<int>(4));
			level.Value.Should().Be(4);
			source.OnNext(this.GetMouseWheelEventArgs(10));
			level.Value.Should().Be(5);
		}

		[Test]
		public void 最大値() {
			var source = new Subject<MouseWheelEventArgs>();
			var level = source.ToZoomLevel(new ReactivePropertySlim<int>(6));
			level.Value.Should().Be(6);
			source.OnNext(this.GetMouseWheelEventArgs(10));
			level.Value.Should().Be(7);
			source.OnNext(this.GetMouseWheelEventArgs(10));
			level.Value.Should().Be(8);
			source.OnNext(this.GetMouseWheelEventArgs(10));
			level.Value.Should().Be(8);
			source.OnNext(this.GetMouseWheelEventArgs(10));
			level.Value.Should().Be(8);
			source.OnNext(this.GetMouseWheelEventArgs(-10));
			level.Value.Should().Be(7);
			source.OnNext(this.GetMouseWheelEventArgs(10));
			level.Value.Should().Be(8);
			source.OnNext(this.GetMouseWheelEventArgs(10));
			level.Value.Should().Be(8);
		}

		[Test]
		public void 最小値() {
			var source = new Subject<MouseWheelEventArgs>();
			var level = source.ToZoomLevel(new ReactivePropertySlim<int>(3));
			level.Value.Should().Be(3);
			source.OnNext(this.GetMouseWheelEventArgs(-10));
			level.Value.Should().Be(2);
			source.OnNext(this.GetMouseWheelEventArgs(-10));
			level.Value.Should().Be(1);
			source.OnNext(this.GetMouseWheelEventArgs(-10));
			level.Value.Should().Be(1);
			source.OnNext(this.GetMouseWheelEventArgs(-10));
			level.Value.Should().Be(1);
			source.OnNext(this.GetMouseWheelEventArgs(10));
			level.Value.Should().Be(2);
			source.OnNext(this.GetMouseWheelEventArgs(-10));
			level.Value.Should().Be(1);
			source.OnNext(this.GetMouseWheelEventArgs(-10));
			level.Value.Should().Be(1);
		}

		/// <summary>
		/// Delta値を指定して<see cref="MouseWheelEventArgs"/>のインスタンスを生成します。
		/// </summary>
		/// <param name="delta">Delta値</param>
		/// <returns><see cref="MouseWheelEventArgs"/>インスタンス</returns>
		private MouseWheelEventArgs GetMouseWheelEventArgs(int delta) {
			var ea = new MouseWheelEventArgs(ObjectCreator.MouseDevice(), 0, delta) {
				RoutedEvent = ObjectCreator.RoutedEvent()
			};
			return ea;
		}
	}
}
