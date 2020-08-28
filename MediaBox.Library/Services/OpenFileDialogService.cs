
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.WindowsAPICodePack.Dialogs;

using SandBeige.MediaBox.Composition.Interfaces.Services;
using SandBeige.MediaBox.Composition.Interfaces.Services.Objects;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Library.Services {
	public class OpenFileDialogService : IOpenFileDialogService {
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
		/// フィルター
		/// </summary>
		public IEnumerable<FileDialogFilter> Filters {
			get;
			set;
		} = Array.Empty<FileDialogFilter>();

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
		public string? FileName {
			get;
			private set;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public IEnumerable<string>? FileNames {
			get;
			private set;
		}

		public bool ShowDialog() {
			var dialog = new CommonOpenFileDialog {
				Title = this.Title,
				InitialDirectory = this.InitialDirectory,
				Multiselect = this.MultiSelect
			};
			dialog.Filters.AddRange(this.Filters.Select(x => new CommonFileDialogFilter(x.RawDisplayName, x.ExtensionList)));
			var ret = dialog.ShowDialog();
			if (ret != CommonFileDialogResult.Ok) {
				return false;
			}
			this.FileName = dialog.FileName;
			this.FileNames = dialog.FileNames;
			return true;
		}
	}
}
