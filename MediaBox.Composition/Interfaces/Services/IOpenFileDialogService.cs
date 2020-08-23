using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Interfaces.Services.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces.Services {
	public interface IOpenFileDialogService {
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
		/// フィルター
		/// </summary>
		public IEnumerable<FileDialogFilter> Filters {
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
		/// ファイル名
		/// </summary>
		public string FileName {
			get;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public IEnumerable<string> FileNames {
			get;
		}

		public bool ShowDialog();
	}
}
