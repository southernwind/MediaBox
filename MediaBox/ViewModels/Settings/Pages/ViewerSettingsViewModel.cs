namespace SandBeige.MediaBox.ViewModels.Settings.Pages {
	/// <summary>
	/// 表示設定ViewModel
	/// </summary>
	internal class ViewerSettingsViewModel : ViewModelBase, ISettingsViewModel {
		/// <summary>
		/// 設定名
		/// </summary>
		public string Name {
			get;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ViewerSettingsViewModel() {
			this.Name = "表示設定";
		}
	}
}
