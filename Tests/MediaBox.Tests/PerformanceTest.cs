using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Tests.ViewModels;

namespace SandBeige.MediaBox.Tests {
	[TestFixture]
	internal class PerformanceTest : ViewModelTestClassBase {
		[Test]
		public void CreateMediaFileViewModel() {
			// TODO : パフォーマンステストは環境によって結果が左右される。どうしよう？
			var sw = new Stopwatch();
			sw.Start();
			var models = new ObservableCollection<IMediaFileModel>(Enumerable.Range(0, 10000).Select(x => this.MediaFactory.Create(Path.Combine(this.TestDataDir, $"image{x}.jpg"))));
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			(sw.ElapsedMilliseconds < 300).IsTrue();
			sw.Reset();
			sw.Restart();
			models.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create);
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			(sw.ElapsedMilliseconds < 300).IsTrue();
		}
	}
}
