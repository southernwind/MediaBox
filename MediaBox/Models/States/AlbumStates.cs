
using System;
using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings.Objects;
using SandBeige.MediaBox.Models.Album.History.Creator;

namespace SandBeige.MediaBox.Models.States {
	/// <summary>
	/// アルバムの状態
	/// </summary>
	public class AlbumStates : SettingsBase {
		/// <summary>
		/// アルバムヒストリー
		/// </summary>
		public SettingsCollection<IAlbumCreator> AlbumHistory {
			get;
		} = new SettingsCollection<IAlbumCreator>(Array.Empty<IAlbumCreator>());

		/// <summary>
		/// カレント条件
		/// </summary>
		public SettingsItemWithKey<string, int?> CurrentFilteringCondition {
			get;
		} = new SettingsItemWithKey<string, int?>(Array.Empty<KeyValuePair<string, int?>>(), null);

		/// <summary>
		/// ソート設定
		/// </summary>
		public SettingsItemWithKey<string, SortDescriptionParams[]> SortDescriptions {
			get;
		} = new SettingsItemWithKey<string, SortDescriptionParams[]>(Array.Empty<KeyValuePair<string, SortDescriptionParams[]>>(), Array.Empty<SortDescriptionParams>());

		/// <summary>
		/// フィルター条件リスト
		/// </summary>
		public SettingsCollection<int> FilteringConditions {
			get;
		} = new SettingsCollection<int>(Array.Empty<int>());
	}
}
