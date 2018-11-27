using Microsoft.Maps.MapControl.WPF;
using SandBeige.MediaBox.ViewModels.Album;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SandBeige.MediaBox.Views.Media.Map {
	/// <summary>
	/// MediaList.xaml の相互作用ロジック
	/// </summary>
	public partial class MediaList : UserControl {
		public MediaList() {
			InitializeComponent();
		}

		protected override void OnRender(DrawingContext drawingContext) {
			var mfvm = (AlbumViewModel)DataContext;
			mfvm.Map.Value = (MapCore)((Panel)this.Content).Children.OfType<object>().Single(x => x is MapCore);
		}
	}
}
