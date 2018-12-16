using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Views.Media.Map {
	/// <summary>
	/// MediaList.xaml の相互作用ロジック
	/// </summary>
	public partial class MediaList {
		public MediaList() {
			this.InitializeComponent();
		}

		protected override void OnRender(DrawingContext drawingContext) {
			var map = (MapCore)((Panel)this.Content).Children.OfType<object>().Single(x => x is MapCore);;
			if (this.DataContext is AlbumViewModel avm) {
				avm.Map.Value = map;
			}

			var rectangle = new Rectangle {
				Fill = Brushes.CadetBlue,
				Width = 100,
				Height = 100
			};
			map.Children.Add(rectangle);
			map.MouseMove += (sender, e) => {
				var vp = map.ViewportPointToLocation(e.GetPosition(map));
				MapLayer.SetPosition(rectangle, vp);
			};
			map.MouseDoubleClick += (sender, e) => {
				var ml = MapLayer.GetPosition(rectangle);
			};
		}
	}
}
