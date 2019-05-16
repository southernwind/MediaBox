﻿using System.ComponentModel;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.Tests.Models.Album.Sort {
	internal class SortDescriptionManagerTest : ModelTestClassBase {

		[Test]
		public void 設定値読み込み() {
			this.Settings.GeneralSettings.SortDescriptions.Value = new[] {
				new SortDescriptionParams(nameof(IMediaFileModel.FileName),ListSortDirection.Descending),
				new SortDescriptionParams(nameof(IMediaFileModel.FilePath),ListSortDirection.Ascending),
			};
			var sdm = new SortDescriptionManager();
			var fn = sdm.SortItems.Single(x => x.Key == nameof(IMediaFileModel.FileName));
			var fp = sdm.SortItems.Single(x => x.Key == nameof(IMediaFileModel.FilePath));
			var fs = sdm.SortItems.Single(x => x.Key == nameof(IMediaFileModel.FileSize));
			fn.Enabled.IsTrue();
			fn.Direction.Is(ListSortDirection.Descending);
			fp.Enabled.IsTrue();
			fp.Direction.Is(ListSortDirection.Ascending);
			fs.Enabled.IsFalse();
			fs.Direction.Is(ListSortDirection.Ascending);
		}

		[Test]
		public void 更新() {
			var sdm = new SortDescriptionManager();

			var fn = sdm.SortItems.Single(x => x.Key == nameof(IMediaFileModel.FileName));
			this.Settings.GeneralSettings.SortDescriptions.Value.Is();
			fn.Enabled = true;
			this.Settings.GeneralSettings.SortDescriptions.Value.Is(new SortDescriptionParams(nameof(IMediaFileModel.FileName), ListSortDirection.Ascending));
			fn.Direction = ListSortDirection.Descending;
			this.Settings.GeneralSettings.SortDescriptions.Value.Is(new SortDescriptionParams(nameof(IMediaFileModel.FileName), ListSortDirection.Descending));
		}
	}
}
