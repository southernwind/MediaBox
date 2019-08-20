using System;
using System.ComponentModel;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.Tests.Models.Album.Sort {
	internal class SortDescriptionManagerTest : ModelTestClassBase {

		[Test]
		public void 設定値読み込み() {
			this.States.AlbumStates.SortConditions.SetValue(new[] {
				new RestorableSortObject("name1", new[] {
					new SortItemCreator(SortItemKeys.FileName),
					new SortItemCreator(SortItemKeys.CreationTime, ListSortDirection.Descending)
				}),
				new RestorableSortObject("name2", new[] {
					new SortItemCreator(SortItemKeys.LastAccessTime, ListSortDirection.Descending),
					new SortItemCreator(SortItemKeys.FileSize, ListSortDirection.Descending)
				})
			});
			this.States.AlbumStates.CurrentSortCondition["main"] = new RestorableSortObject("name1", new[] {
					new SortItemCreator(SortItemKeys.FileName),
					new SortItemCreator(SortItemKeys.CreationTime, ListSortDirection.Descending)
				});
			using var sdm = new SortDescriptionManager("main");
			sdm.SortConditions.Count.Is(2);
			var s1 = sdm.SortConditions.First();
			var s2 = sdm.SortConditions.Skip(1).First();
			s1.DisplayName.Value.Is("name1");
			s1.SortItemCreators
				.Select(x => (x.SortItemKey, x.Direction))
				.Is(
					(SortItemKeys.FileName, ListSortDirection.Ascending),
					(SortItemKeys.CreationTime, ListSortDirection.Descending));
			s2.DisplayName.Value.Is("name2");
			s2.SortItemCreators
				.Select(x => (x.SortItemKey, x.Direction))
				.Is(
					(SortItemKeys.LastAccessTime, ListSortDirection.Descending),
					(SortItemKeys.FileSize, ListSortDirection.Descending));

			sdm.CurrentSortCondition.Value.Is(s1);
		}

		[Test]
		public void ソート設定追加() {
			using var sdm = new SortDescriptionManager("main");

			this.States.AlbumStates.SortConditions.SetValue(Array.Empty<RestorableSortObject>());
			sdm.SortConditions.IsEmpty();
			sdm.AddCondition();
			sdm.SortConditions.Count.Is(1);
			this.States.AlbumStates.SortConditions.Value.Is(sdm.SortConditions.First().RestorableSortObject);
		}
	}
}
