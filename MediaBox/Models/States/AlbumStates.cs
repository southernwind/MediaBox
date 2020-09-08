using System;
using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.History;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Composition.Settings.Objects;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.Models.States {
	/// <summary>
	/// アルバムの状態
	/// </summary>
	public class AlbumStates : SettingsBase, IAlbumStates {
		/// <summary>
		/// アルバムヒストリー
		/// </summary>
		public SettingsCollection<IHistoryObject> AlbumHistory {
			get;
		} = new SettingsCollection<IHistoryObject>(Array.Empty<IHistoryObject>());

		/// <summary>
		/// カレントソート条件
		/// </summary>
		public SettingsItemWithKey<string, ISortObject?> CurrentSortCondition {
			get;
		} = new SettingsItemWithKey<string, ISortObject?>(Array.Empty<KeyValuePair<string, ISortObject?>>(), null);

		/// <summary>
		/// カレントフィルター条件
		/// </summary>
		public SettingsItemWithKey<string, IFilterObject?> CurrentFilteringCondition {
			get;
		} = new SettingsItemWithKey<string, IFilterObject?>(Array.Empty<KeyValuePair<string, IFilterObject?>>(), null);

		/// <summary>
		/// ソート条件リスト
		/// </summary>
		public SettingsCollection<ISortObject> SortConditions {
			get;
		} = new SettingsCollection<ISortObject>(
			new SortObject("ファイルパス", new[] { new SortItemCreator(SortItemKeys.FilePath) }),
			new SortObject("編集日時", new[] { new SortItemCreator(SortItemKeys.ModifiedTime) }),
			new SortObject("評価", new[] { new SortItemCreator(SortItemKeys.Rate) }),
			new SortObject("ファイルサイズ", new[] { new SortItemCreator(SortItemKeys.FileSize) }));

		/// <summary>
		/// フィルター条件リスト
		/// </summary>
		public SettingsCollection<IFilterObject> FilteringConditions {
			get;
		} = new SettingsCollection<IFilterObject>(Array.Empty<IFilterObject>());
	}
}
