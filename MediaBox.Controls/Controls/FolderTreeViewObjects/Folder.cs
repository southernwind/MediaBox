using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

using Livet;

namespace SandBeige.MediaBox.Controls.Controls.FolderTreeViewObjects {
	/// <summary>
	/// フォルダーツリーのフォルダアイテム
	/// </summary>
	public class Folder : NotificationObject, IFolderTreeViewItem {
		/// <summary>
		/// DriveType→string変換辞書
		/// </summary>
		private static readonly Dictionary<DriveType, string> _driveTypes = new Dictionary<DriveType, string> {
			{DriveType.Unknown,"不明" },
			{DriveType.NoRootDirectory,"" },
			{DriveType.Removable,"リムーバブルディスク" },
			{DriveType.Fixed,"ローカルディスク" },
			{DriveType.Network,"ネットワークドライブ" },
			{DriveType.CDRom,"CDドライブ" },
			{DriveType.Ram,"RAMディスク" }
		};

		/// <summary>
		/// ダミー
		/// </summary>
		private static readonly IEnumerable<IFolderTreeViewItem> _dummyChildren = new List<IFolderTreeViewItem> { null };
		/// <summary>
		/// 空
		/// </summary>
		private static readonly IEnumerable<IFolderTreeViewItem> _emptyChildren = new List<IFolderTreeViewItem>();

		private bool _isExpanded;
		private bool _isSelected;
		private IEnumerable<IFolderTreeViewItem> _children;

		/// <summary>
		/// コンストラクタ(ルート用)
		/// </summary>
		internal Folder() {
			this.FolderPath = "";
		}

		/// <summary>
		/// コンストラクタ(内部向け)
		/// </summary>
		/// <param name="folderPath">フォルダパス</param>
		private Folder(string folderPath) {
			this.DisplayName = folderPath.Split('\\').Last();
			this.Icon = IconUtility.GetIcon(folderPath);
			this.FolderPath = $@"{folderPath}\";
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drive">ドライブ情報</param>
		internal Folder(DriveInfo drive) {
			var v = _driveTypes[drive.DriveType];
			if (drive.IsReady) {
				v = string.IsNullOrEmpty(drive.VolumeLabel) ? v : drive.VolumeLabel;
			} else {
				this._children = _emptyChildren;
			}
			this.FolderPath = drive.Name;
			this.Icon = IconUtility.GetIcon(drive.Name);
			this.DisplayName = $"{v}({this.FolderPath.Replace(@"\", "")})";
		}

		/// <summary>
		/// フォルダパス
		/// </summary>
		public string FolderPath {
			get;
		}

		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get;
			set;
		}

		/// <summary>
		/// アイコン
		/// </summary>
		public ImageSource Icon {
			get;
			set;
		}

		/// <summary>
		/// 広がっているか
		/// </summary>
		public bool IsExpanded {
			get {
				return this._isExpanded;
			}
			set {
				if (!this.RaisePropertyChangedIfSet(ref this._isExpanded, value)) {
					return;
				}
				if (this._isExpanded && this._children == null) {
					try {
						this.Children = Directory.EnumerateDirectories(this.FolderPath).Select(x => new Folder(x)).ToList();
					} catch (UnauthorizedAccessException ex) {
						this.Children = new[] { new ErrorItem(ex.Message) };
					}
				}
			}
		}

		/// <summary>
		/// 選択されているか
		/// </summary>
		public bool IsSelected {
			get {
				return this._isSelected;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._isSelected, value);
			}
		}

		/// <summary>
		/// 配下フォルダ
		/// </summary>
		public IEnumerable<IFolderTreeViewItem> Children {
			get {
				return this._children ?? _dummyChildren;
			}
			internal set {
				this.RaisePropertyChangedIfSet(ref this._children, value);
			}
		}

		/// <summary>
		/// 指定フォルダパスの選択
		/// </summary>
		/// <remarks>
		/// 直下のフォルダがフォルダパスに含まれていればフォルダを展開し、子要素の<see cref="Select(string)"/>を呼び出す。
		/// </remarks>
		/// <param name="path">フォルダパス</param>
		public void Select(string path) {
			if (path == null) {
				return;
			}
			if (path.StartsWith(this.FolderPath)) {
				this.IsExpanded = true;
				foreach (var child in this.Children) {
					if (child is Folder folder) {
						folder.Select(path);
					}
				}
			}
			if (path == this.FolderPath) {
				this.IsSelected = true;
			}
		}
	}
}
