using SandBeige.MediaBox.ViewModels.Album.Viewer;

namespace SandBeige.MediaBox.Views.Album.Viewer {
	/// <summary>
	/// Map.xaml の相互作用ロジック
	/// </summary>
	public partial class Map {
		public Map() {
			this.InitializeComponent();

			this.DataContextChanged += (sender, e) => {
				if (e.NewValue is MapViewerViewModel mvm) {
					mvm.Map.Value.MapControl.Value = this.MapControl;
				}
			};
		}
	}
}
