using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Livet;

using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Map;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using Point = System.Windows.Point;

namespace SandBeige.MediaBox.Views.Map {
	/// <summary>
	/// MapControl.xaml の相互作用ロジック
	/// </summary>
	public partial class MapControl : IMapControl {
		public MapControl() {
			this.InitializeComponent();
			this.Collection.CollectionChanged += (sender, e) => {
				var g = 0.1;
				this.Children.Clear();
				this.Collection.ForEach(x => {

					var pin = new Border {
						BorderBrush = new SolidColorBrush(Colors.Red),
						BorderThickness = new Thickness(5),
						Width = 100,
						Height = 100
					};
					g += 0.1;
					pin.DataContext = x;
					Windows.UI.Xaml.Controls.Maps.MapControl.SetLocation(pin, new Windows.Devices.Geolocation.Geopoint(new Windows.Devices.Geolocation.BasicGeoposition {
						Latitude = 34 + g,
						Longitude = 135 + g
					}));
					Windows.UI.Xaml.Controls.Maps.MapControl.SetNormalizedAnchorPoint(pin, new Windows.Foundation.Point(0.5, 0.5));
				});
			};
		}

		public GpsLocation ViewportPointToLocation(Point viewportPoint) {
			throw new NotImplementedException();
		}

		public Point LocationToViewportPoint(GpsLocation location) {
			this.GetOffsetFromLocation(new Geopoint(new BasicGeoposition {
				Latitude = location.Latitude,
				Longitude = location.Longitude
			}), out var point);
			return new Point(point.X, point.Y);
		}

		/// <summary>
		/// 範囲プロパティの値が異常値か否か
		/// </summary>
		public bool HasAreaPropertyError {
			get {
				return false;
			}
		}

		/// <summary>
		/// 準備完了通知
		/// </summary>
		public IObservable<Unit> Ready {
			get {
				return Observable.Return(Unit.Default);
			}
		}

		public IObservable<Unit> ChangeViewArea {
			get {
				return Observable.Return(Unit.Default);
			}
		}

		public ObservableSynchronizedCollection<object> Collection {
			get;
		} = new ObservableSynchronizedCollection<object>();

		/// <summary>
		/// 表示エリア設定
		/// </summary>
		/// <param name="leftTop">左上GPS座標</param>
		/// <param name="rightBottom">右下GPS座標</param>
		/// <param name="paddingPixel">余白(px)</param>
		public void SetViewArea(GpsLocation leftTop, GpsLocation rightBottom, int paddingPixel) {
			// leftTop,rightBottomを受け取り、表示すべき座標の範囲を表示しきれる最も拡大された地図を表示する。
			// 現在の倍率、幅、高さと必要な幅、高さから、倍率を計算して適用する。
			// ※ ZoomLevelは最小0.75,最大21で、ZoomLevelが1上がる毎に表示できる領域が縦横ともに2倍になる。
		}
	}
}
