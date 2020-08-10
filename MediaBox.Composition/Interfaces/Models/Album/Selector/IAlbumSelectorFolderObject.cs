using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Interfaces.Controls;
using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector {
	public interface IAlbumSelectorFolderObject : IFolderTreeViewItem {

		/// <summary>
		/// 子の更新
		/// </summary>
		/// <param name="directories">ツリーに含むディレクトリリスト</param>
		public void Update(IEnumerable<ValueCountPair<string>> directories);
	}
}
