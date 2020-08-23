using System.Collections.Generic;

namespace SandBeige.MediaBox.Composition.Interfaces.Services {
	public interface IFolderSelectionDialogService {
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title {
			get;
			set;
		}

		/// <summary>
		/// 初期ディレクトリ
		/// </summary>
		public string InitialDirectory {
			get;
			set;
		}

		/// <summary>
		/// 複数選択
		/// </summary>
		public bool MultiSelect {
			get;
			set;
		}

		/// <summary>
		/// フォルダ名
		/// </summary>
		public string FolderName {
			get;
		}

		/// <summary>
		/// フォルダ名
		/// </summary>
		public IEnumerable<string> FolderNames {
			get;
		}

		public bool ShowDialog();
	}
}
