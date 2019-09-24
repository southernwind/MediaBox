
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Livet;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Tests.Implements;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Map {
	[Apartment(ApartmentState.STA)]
	internal class MapModelTest : ModelTestClassBase {
		[Test]
		public void MediaFiles() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var map = new MapModel(osc, rp);
			map.Items.Is(osc);
			map.CurrentMediaFiles.Is(rp);
		}

		[Test]
		public void MapApiKey() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var map = new MapModel(osc, rp);
			this.Settings.GeneralSettings.BingMapApiKey.Value = "abcdefghijk";
			map.BingMapApiKey.Value.Is("abcdefghijk");
		}

		[Test]
		public void MapPinSize() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var map = new MapModel(osc, rp);
			this.Settings.GeneralSettings.MapPinSize.Value = 193;
			map.MapPinSize.Value.Is(193);
		}

		[Test]
		public async Task アイテム追加削除によるマップの表示エリア変更() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var map = new MapModel(osc, rp);
			var mc = map.MapControl.Value;

			mc.North.Is(0);
			mc.West.Is(0);
			mc.East.Is(0);
			mc.South.Is(0);

			var (_, m1) = this.Register(this.TestFiles.Image1Jpg);
			osc.Add(m1);

			await Task.Delay(200);
			Assert.AreEqual(34.697419, mc.North, 0.001);
			Assert.AreEqual(135.533553, mc.West, 0.001);
			Assert.AreEqual(135.533553, mc.East, 0.001);
			Assert.AreEqual(34.697419, mc.South, 0.001);

			var (_, m2) = this.Register(this.TestFiles.Image2Jpg);
			osc.Add(m2);

			await Task.Delay(200);
			Assert.AreEqual(34.697419, mc.North, 0.001);
			Assert.AreEqual(-132.183486, mc.West, 0.001);
			Assert.AreEqual(135.533553, mc.East, 0.001);
			Assert.AreEqual(-35.184364, mc.South, 0.001);

			osc.Remove(m1);

			await Task.Delay(200);
			Assert.AreEqual(-35.184364, mc.North, 0.001);
			Assert.AreEqual(-132.183486, mc.West, 0.001);
			Assert.AreEqual(-132.183486, mc.East, 0.001);
			Assert.AreEqual(-35.184364, mc.South, 0.001);
		}

		[Test]
		public async Task MapViewer表示中の場合マップ範囲が変化しない() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var map = new MapModel(osc, rp);
			var mc = map.MapControl.Value;

			mc.North.Is(0);
			mc.West.Is(0);
			mc.East.Is(0);
			mc.South.Is(0);

			var (_, m1) = this.Register(this.TestFiles.Image1Jpg);
			osc.Add(m1);

			await Task.Delay(200);
			mc.North.Is(0);
			mc.West.Is(0);
			mc.East.Is(0);
			mc.South.Is(0);

			var (_, m2) = this.Register(this.TestFiles.Image2Jpg);
			osc.Add(m2);

			await Task.Delay(200);
			mc.North.Is(0);
			mc.West.Is(0);
			mc.East.Is(0);
			mc.South.Is(0);

			osc.Remove(m1);

			await Task.Delay(200);
			mc.North.Is(0);
			mc.West.Is(0);
			mc.East.Is(0);
			mc.South.Is(0);
		}

		[Test]
		public void マップ上をマウスダブルクリックで決定通知() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var map = new MapModel(osc, rp);
			var mc = (MapControlForTest)map.MapControl.Value;
			var count = 0;
			map.OnDecide.Subscribe(_ => {
				count++;
			});
			count.Is(0);
			mc.OnMouseDoubleClick(mc, new MouseButtonEventArgs(ObjectCreator.MouseDevice(), 0, MouseButton.Left) {
				RoutedEvent = ObjectCreator.RoutedEvent()
			});
			count.Is(1);
			mc.OnMouseDoubleClick(mc, new MouseButtonEventArgs(ObjectCreator.MouseDevice(), 0, MouseButton.Left) {
				RoutedEvent = ObjectCreator.RoutedEvent()
			});
			count.Is(2);
		}

		[Test]
		public async Task ピングルーピング() {
			// マップコントロールが絡んでいてかなり複雑なので、実行して例外が出ない確認のみ。
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var map = new MapModel(osc, rp);
			var (_, m1) = this.Register(this.TestFiles.Image1Jpg);
			var (_, m2) = this.Register(this.TestFiles.Image2Jpg);
			var (_, m3) = this.Register(this.TestFiles.Image3Jpg);
			osc.AddRange(m1, m2, m3);
			map.CurrentMediaFiles.Value = new[] { m1 };
			map.IgnoreMediaFiles.Value = new[] { m3 };
			foreach (var i in Enumerable.Range(1, 20)) {
				if (map.ItemsForMapView.Value.Count() != 0) {
					break;
				}
				await Task.Delay(100);
			}

			map.ItemsForMapView.Value.Count().Is(1);
		}
	}
}
