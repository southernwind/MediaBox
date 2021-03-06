using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Maps.MapControl.WPF;

using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces.Models.Map;

namespace SandBeige.MediaBox.Views.Map {
	/// <summary>
	/// MapControl.xaml の相互作用ロジック
	/// </summary>
	public partial class MapControl : IMapControl {
		/// <summary>
		/// 最東座標
		/// </summary>
		public double East {
			get {
				return this.BoundingRectangle.East;
			}
		}

		/// <summary>
		/// 最西座標
		/// </summary>
		public double West {
			get {
				return this.BoundingRectangle.West;
			}
		}

		/// <summary>
		/// 最北座標
		/// </summary>
		public double North {
			get {
				return this.BoundingRectangle.North;
			}
		}

		/// <summary>
		/// 最南座標
		/// </summary>
		public double South {
			get {
				return this.BoundingRectangle.South;
			}
		}

		public MapControl() {
			this.InitializeComponent();
		}

		/// <summary>
		/// 範囲プロパティの値が異常値か否か
		/// </summary>
		public bool HasAreaPropertyError {
			get {
				var result = false;
				this.Dispatcher.Invoke(() => {
					result =
						this.BoundingRectangle.Center.Longitude > this.Center.Longitude + 0.00001 ||
						this.BoundingRectangle.Center.Longitude < this.Center.Longitude - 0.00001 ||
						this.BoundingRectangle.West > this.BoundingRectangle.East;
				});
				return result;
			}
		}

		/// <summary>
		/// 準備完了通知
		/// </summary>
		public IObservable<Unit> Ready {
			get {
				return Observable.FromEvent<SizeChangedEventHandler, SizeChangedEventArgs>(
						h => (sender, e) => h(e),
						h => this.SizeChanged += h,
						h => this.SizeChanged -= h)
					.ToUnit()
					.TakeUntil(x => this.BoundingRectangle.Height != 0);
			}
		}

		/// <summary>
		/// 表示エリア設定
		/// </summary>
		/// <param name="leftTop">左上GPS座標</param>
		/// <param name="rightBottom">右下GPS座標</param>
		/// <param name="paddingPixel">余白(px)</param>
		public void SetViewArea(Location leftTop, Location rightBottom, int paddingPixel) {
			// leftTop,rightBottomを受け取り、表示すべき座標の範囲を表示しきれる最も拡大された地図を表示する。
			// 現在の倍率、幅、高さと必要な幅、高さから、倍率を計算して適用する。
			// ※ ZoomLevelは最小0.75,最大21で、ZoomLevelが1上がる毎に表示できる領域が縦横ともに2倍になる。
			this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
				this.Center = new Location((leftTop.Latitude / 2) + (rightBottom.Latitude / 2), (leftTop.Longitude / 2) + (rightBottom.Longitude / 2));
				this.ZoomLevel = 5;
				// 現在の描画範囲の幅と高さを取得
				var loc = this.ViewportPointToLocation(new Point(paddingPixel, paddingPixel));
				var currentWidth = Math.Abs(loc.Latitude - this.Center.Latitude) * 2;
				var currentHeight = Math.Abs(loc.Longitude - this.Center.Longitude) * 2;
				// 必要な幅と高さを取得
				var requiredWidth = Math.Abs(rightBottom.Latitude - leftTop.Latitude);
				var requiredHeight = Math.Abs(rightBottom.Longitude - leftTop.Longitude);

				// 倍率を計算
				var magnification = Math.Max(requiredWidth / currentWidth, requiredHeight / currentHeight);

				// 倍率をZoomLevelに変換して加算
				this.ZoomLevel += Math.Log(1 / magnification, 2);
			}));
		}
	}
}
