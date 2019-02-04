using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;

namespace SandBeige.MediaBox.Controls.Controls.FolderTreeViewObjects {
	public class ErrorItem : IFolderTreeViewItem {
		public string DisplayName {
			get;
		}

		public bool IsSelected {
			get;
			set;
		}

		public bool IsExpanded {
			get;
			set;
		}

		public ImageSource Icon {
			get;
		}

		public IEnumerable<IFolderTreeViewItem> Children {
			get;
		} = Array.Empty<IFolderTreeViewItem>();

		public ErrorItem(string displayName) {
			this.DisplayName = displayName;
			this.Icon = SystemIcons.Error.ToImageSource();
		}
	}
}
