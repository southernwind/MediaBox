using System.Collections.Generic;

namespace SandBeige.MediaBox.ViewModels.Media.ThumbnailCreator {
	/// <summary>
	/// サムネイル作成ViewModel
	/// </summary>
	internal class ThumbnailCreatorViewModel : ViewModelBase {

		/// <summary>
		/// サムネイル作成対象ファイルリスト
		/// </summary>
		public IEnumerable<VideoFileViewModel> Files {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="files">サムネイル作成対象ファイルリスト</param>
		public ThumbnailCreatorViewModel(IEnumerable<VideoFileViewModel> files) {
			this.Files = files;
		}
	}
}
