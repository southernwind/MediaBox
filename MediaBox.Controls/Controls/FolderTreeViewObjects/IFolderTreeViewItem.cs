using System.Windows.Media;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Controls.Controls.FolderTreeViewObjects {
	/// <summary>
	/// フォルダーツリーアイテムインターフェイス
	/// </summary>
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

		/// <summary>
		/// 子
		/// </summary>
		ReactiveCollection<IFolderTreeViewItem> Children {
			get;
		}
	}

}
