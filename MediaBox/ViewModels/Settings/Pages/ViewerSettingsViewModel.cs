
using Reactive.Bindings;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

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
		/// メディアファイル表示コントロールXAML
		/// </summary>
		public IReactiveProperty<string> MediaFileViewerControlXaml {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 適用コマンド
		/// </summary>
		public ReactiveCommand ApplyCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ViewerSettingsViewModel() {
			this.Name = "表示設定";

			var sampleModel = Get.Instance<MediaFactory>().Create(@"C:\Users\admin\Pictures\2018-04\IMG_4801.JPG");
			sampleModel.UpdateFileInfo();
			sampleModel.CreateThumbnailIfNotExists();
			this.MediaFileViewerControlXaml.Value = this.Settings.ViewerSettings.MediaFileViewerControlXaml.Value;
			this.ApplyCommand.Subscribe(() => {
				this.Settings.ViewerSettings.MediaFileViewerControlXaml.Value = this.MediaFileViewerControlXaml.Value;
			});
		}
	}
}
