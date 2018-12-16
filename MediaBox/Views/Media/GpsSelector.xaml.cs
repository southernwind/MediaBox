using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Views.Media {
	/// <summary>
	/// GpsSelector.xaml の相互作用ロジック
	/// </summary>
	public partial class GpsSelector {
		public GpsSelector() {
			this.InitializeComponent();
		}

		protected override void OnRender(DrawingContext drawingContext) {
			var map = (MapCore)((Panel)this.Content).Children.OfType<object>().Single(x => x is MapCore);
			if (this.DataContext is GpsSelectorViewModel vm) {
				vm.Map.Value = map;
				var pin = (Grid)map.Children.OfType<object>().Single(x => x is Grid);
				map.MouseMove += (sender, e) => {
					var vp = map.ViewportPointToLocation(e.GetPosition(map));
					vm.Latitude.Value = vp.Latitude;
					vm.Longitude.Value = vp.Longitude;
				};
				map.MouseDoubleClick += (sender, e) => {
					if (vm.TargetFiles.Value == null) {
						return;
					}
					if(MessageBox.Show("GPSを設定します。", "確認", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK){
						vm.SetGpsCommand.Execute();
					}
				};
			}
		}
	}
}