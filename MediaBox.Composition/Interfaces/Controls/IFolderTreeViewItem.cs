using System.Windows.Media;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Controls {
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
		/// フォルダパス
		/// </summary>
		string FolderPath {
			get;
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

		/// <summary>
		/// 指定フォルダパスの選択
		/// </summary>
		/// <param name="path">フォルダパス</param>
		void Select(string path);
	}

}
