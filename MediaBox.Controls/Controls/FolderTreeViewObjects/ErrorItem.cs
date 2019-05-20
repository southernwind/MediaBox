using System.Drawing;
using System.Windows.Media;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Controls.Controls.FolderTreeViewObjects {
	/// <summary>
	/// フォルダーツリー展開失敗時の代替エラーアイテム
	/// </summary>
	public class ErrorItem : IFolderTreeViewItem {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get;
		}

		/// <summary>
		/// 選択されているか
		/// </summary>
		public bool IsSelected {
			get;
			set;
		}

		/// <summary>
		/// 広がっているか
		/// </summary>
		public bool IsExpanded {
			get;
			set;
		}

		/// <summary>
		/// フォルダパス
		/// </summary>
		public string FolderPath {
			get;
			set;
		}

		/// <summary>
		/// アイコン
		/// </summary>
		public ImageSource Icon {
			get;
		}

		/// <summary>
		/// 子
		/// </summary>
		public ReactiveCollection<IFolderTreeViewItem> Children {
			get;
		} = new ReactiveCollection<IFolderTreeViewItem>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="displayName">表示名</param>
		public ErrorItem(string displayName) {
			this.DisplayName = displayName;
			this.Icon = SystemIcons.Error.ToImageSource();
		}

		/// <summary>
		/// 指定フォルダパスの選択
		/// </summary>
		/// <param name="path">フォルダパス</param>
		public void Select(string path) {
			// パスがないので何もしない
		}
	}
}
