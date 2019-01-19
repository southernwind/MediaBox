namespace SandBeige.MediaBox.ViewModels.Settings.Pages {
	internal class PathSettingsViewModel : ViewModelBase, ISettingsViewModel {
		public string Name {
			get;
		}

		public PathSettingsViewModel() {
			this.Name = "パス設定";
		}
	}
}
