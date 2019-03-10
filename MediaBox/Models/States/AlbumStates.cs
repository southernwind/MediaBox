
using System;

using SandBeige.MediaBox.Composition.Settings.Objects;
using SandBeige.MediaBox.Models.Album.History;

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
		public SettingsItem<int?> CurrentFilteringCondition {
			get;
		} = new SettingsItem<int?>(null);

		/// <summary>
		/// フィルター条件リスト
		/// </summary>
		public SettingsCollection<int> FilteringConditions {
			get;
		} = new SettingsCollection<int>(Array.Empty<int>());
	}
}
