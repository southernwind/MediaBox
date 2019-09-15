using SandBeige.MediaBox.ViewModels.Media.ThumbnailCreator;

namespace SandBeige.MediaBox.Views.Media.ThumbnailCreator {
	/// <summary>
	/// ThumbnailCreatorWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class ThumbnailCreatorWindow {
		public ThumbnailCreatorWindow() {
			this.InitializeComponent();
			this.DataContextChanged += (sender, e) => {
				if (e.NewValue is ThumbnailCreatorViewModel vm) {
					vm.MediaElementControl.Value = this.Media;
				}
			};
		}

	}
}
