using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Media {
	public interface IMediaFileProperty {
		bool HasMultipleValues {
			get;
		}
		ValueCountPair<string?> RepresentativeValue {
			get;
		}
		string Title {
			get;
		}
		IEnumerable<ValueCountPair<string?>> Values {
			get;
		}
	}
}