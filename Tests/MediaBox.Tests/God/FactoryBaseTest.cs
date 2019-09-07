using System;

using NUnit.Framework;

using SandBeige.MediaBox.God;

namespace SandBeige.MediaBox.Tests.God {
	[TestFixture]
	internal class FactoryBaseTest {

		[Test]
		public void 同一キーから生成されたインスタンスは同一参照() {
			var fb = new FactoryBaseForTest();
			var sw1 = fb.Create("A");
			var sw2 = fb.Create("B");
			var sw3 = fb.Create("B");
			var sw4 = fb.Create("A");

			fb.Count.Is(2);
			sw1.Is(sw4);
			sw2.Is(sw3);
			sw1.IsNot(sw2);
		}

		[Test]
		public void キーがnullの場合defaultを返す() {
			// TODO:null許容するオプションを作っても良いかも？
			var fb = new FactoryBaseForTest();
			var sw1 = fb.Create(null);

			sw1.IsNull();
			fb.Count.Is(0);
		}

		private class FactoryBaseForTest : FactoryBase<string, StringWrap> {
			/// <summary>
			/// インスタンス作成回数
			/// </summary>
			public int Count {
				get;
				private set;
			}
			public StringWrap Create(string key) {
				return this.Create<string, StringWrap>(key);
			}

			protected override StringWrap CreateInstance<TKey, TValue>(TKey key) {
				this.Count++;
				return new StringWrap(key);
			}
		}

		internal class StringWrap : IDisposable {
			public string Text {
				get;
				private set;
			}

			public StringWrap(string text) {
				this.Text = text;
			}

			public void Dispose() {
				this.Text = null;
			}
		}
	}
}
