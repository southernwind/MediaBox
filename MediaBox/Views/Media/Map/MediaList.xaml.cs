using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
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
			if (this.DataContext is AlbumViewModel avm) {
				avm.Map.Value = (MapCore)((Panel)this.Content).Children.OfType<object>().Single(x => x is MapCore);
			}
		}
	}
}
