using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal abstract class FilterCreatorTestClassBase : ModelTestClassBase {
		protected abstract void SetDatabaseRecord();
		protected abstract IEnumerable<IMediaFileModel> CreateModels();
	}
}
