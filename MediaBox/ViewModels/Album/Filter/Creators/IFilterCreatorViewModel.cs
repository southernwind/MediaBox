

using MahApps.Metro.IconPacks;

namespace SandBeige.MediaBox.ViewModels.Album.Filter.Creators {
	/// <summary>
	/// フィルター作成VM
	/// </summary>
	public interface IFilterCreatorViewModel {
		/// <summary>
		/// 表示名
		/// </summary>
		string Title {
			get;
		}

		/// <summary>
		/// アイコン
		/// </summary>
		PackIconBase Icon {
			get;
		}
	}
}
