using SandBeige.MediaBox.Base;

namespace SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow.Pages {
	internal class PathSettingsViewModel : ViewModelBase, ISettingsViewModel {
		public string Name {
			get;
		}

		public PathSettingsViewModel() {
			this.Name = "パス設定";
		}
	}
}
