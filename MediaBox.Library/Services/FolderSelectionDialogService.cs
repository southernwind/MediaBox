using System.Collections.Generic;

using Microsoft.WindowsAPICodePack.Dialogs;

using SandBeige.MediaBox.Composition.Interfaces.Services;

namespace SandBeige.MediaBox.Library.Services {
	public class FolderSelectionDialogService : IFolderSelectionDialogService {
		/// <summary>
		/// タイトル
		/// </summary>
		public string? Title {
			get;
			set;
		}

		/// <summary>
		/// 初期ディレクトリ
		/// </summary>
		public string? InitialDirectory {
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
		public string? FolderName {
			get;
			private set;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public IEnumerable<string>? FolderNames {
			get;
			private set;
		}

		public bool ShowDialog() {
			var dialog = new CommonOpenFileDialog {
				IsFolderPicker = true,
				Title = this.Title,
				InitialDirectory = this.InitialDirectory,
				Multiselect = this.MultiSelect
			};
			var ret = dialog.ShowDialog();
			if (ret != CommonFileDialogResult.Ok) {
				return false;
			}
			this.FolderName = dialog.FileName;
			this.FolderNames = dialog.FileNames;
			return true;
		}
	}
}
