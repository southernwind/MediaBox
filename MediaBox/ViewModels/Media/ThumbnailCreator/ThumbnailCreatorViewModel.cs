using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.ViewModels.Media.ThumbnailCreator {
	/// <summary>
	/// サムネイル作成ViewModel
	/// </summary>
	internal class ThumbnailCreatorViewModel : ViewModelBase {

		/// <summary>
		/// サムネイル作成対象ファイルリスト
		/// </summary>
		public IEnumerable<IMediaFileViewModel> Files {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="files">サムネイル作成対象ファイルリスト</param>
		public ThumbnailCreatorViewModel(IEnumerable<IMediaFileViewModel> files) {
			this.Files = files;
		}
	}
}
