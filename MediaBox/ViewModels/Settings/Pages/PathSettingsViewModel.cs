namespace SandBeige.MediaBox.ViewModels.Settings.Pages {
	/// <summary>
	/// パス設定ViewModel
	/// </summary>
	internal class PathSettingsViewModel : ViewModelBase, ISettingsViewModel {
		/// <summary>
		/// 設定名
		/// </summary>
		public string Name {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PathSettingsViewModel() {
			this.Name = "パス設定";
		}
	}
}
