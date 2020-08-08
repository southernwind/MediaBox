using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.History;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.States {
	/// <summary>
	/// アルバムの状態
	/// </summary>
	public interface IAlbumStates : ISettingsBase {
		/// <summary>
		/// アルバムヒストリー
		/// </summary>
		public SettingsCollection<IHistoryObject> AlbumHistory {
			get;
		}

		/// <summary>
		/// カレントソート条件
		/// </summary>
		public SettingsItemWithKey<string, ISortObject> CurrentSortCondition {
			get;
		}

		/// <summary>
		/// カレントフィルター条件
		/// </summary>
		public SettingsItemWithKey<string, IFilterObject> CurrentFilteringCondition {
			get;
		}

		/// <summary>
		/// ソート条件リスト
		/// </summary>
		public SettingsCollection<ISortObject> SortConditions {
			get;
		}
		/// <summary>
		/// フィルター条件リスト
		/// </summary>
		public SettingsCollection<IFilterObject> FilteringConditions {
			get;
		}
	}
}
