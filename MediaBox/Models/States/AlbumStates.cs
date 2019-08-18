using System;
using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Settings.Objects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.History.Creator;
using SandBeige.MediaBox.Models.Album.Sort;

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
		/// カレントソート条件
		/// </summary>
		public SettingsItemWithKey<string, RestorableSortObject> CurrentSortCondition {
			get;
		} = new SettingsItemWithKey<string, RestorableSortObject>(Array.Empty<KeyValuePair<string, RestorableSortObject>>(), null);

		/// <summary>
		/// カレントフィルター条件
		/// </summary>
		public SettingsItemWithKey<string, RestorableFilterObject> CurrentFilteringCondition {
			get;
		} = new SettingsItemWithKey<string, RestorableFilterObject>(Array.Empty<KeyValuePair<string, RestorableFilterObject>>(), null);

		/// <summary>
		/// ソート条件リスト
		/// </summary>
		public SettingsCollection<RestorableSortObject> SortConditions {
			get;
		} = new SettingsCollection<RestorableSortObject>(
			new RestorableSortObject("ファイルパス", new[] { new SortItemCreator(SortItemKeys.FilePath) }),
			new RestorableSortObject("編集日時", new[] { new SortItemCreator(SortItemKeys.ModifiedTime) }),
			new RestorableSortObject("評価", new[] { new SortItemCreator(SortItemKeys.Rate) }),
			new RestorableSortObject("ファイルサイズ", new[] { new SortItemCreator(SortItemKeys.FileSize) }));

		/// <summary>
		/// フィルター条件リスト
		/// </summary>
		public SettingsCollection<RestorableFilterObject> FilteringConditions {
			get;
		} = new SettingsCollection<RestorableFilterObject>(Array.Empty<RestorableFilterObject>());
	}
}
