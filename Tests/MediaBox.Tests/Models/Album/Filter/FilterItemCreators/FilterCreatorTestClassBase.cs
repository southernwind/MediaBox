using System.Collections.Generic;
using System.Linq;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal abstract class FilterCreatorTestClassBase : ModelTestClassBase {
		protected IEnumerable<MediaFile>? TestTableData;
		protected IEnumerable<IMediaFileModel>? TestModelData;

		protected void CreateModels() {
			this.TestModelData = this.TestTableData?.Select(r => r.ToModel());
		}
	}
}
