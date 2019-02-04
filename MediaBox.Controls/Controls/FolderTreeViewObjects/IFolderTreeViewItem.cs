using System.Collections.Generic;
using System.Windows.Media;

namespace SandBeige.MediaBox.Controls.Controls.FolderTreeViewObjects {
	public interface IFolderTreeViewItem {
		/// <summary>
		/// 表示名
		/// </summary>
		string DisplayName {
			get;
		}

		/// <summary>
		/// 選択されているか
		/// </summary>
		bool IsSelected {
			get;
			set;
		}


		/// <summary>
		/// 広がっているか
		/// </summary>
		bool IsExpanded {
			get;
			set;
		}

		/// <summary>
		/// アイコンイメージ
		/// </summary>
		ImageSource Icon {
			get;
		}

		IEnumerable<IFolderTreeViewItem> Children {
			get;
		}
	}

}
