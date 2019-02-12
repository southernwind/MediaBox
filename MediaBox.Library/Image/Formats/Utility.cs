using System.Collections.Generic;
using System.Linq;

using MetadataExtractor;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Image.Formats {
	internal static class Utility {
		public static IEnumerable<TitleValuePair<IEnumerable<TitleValuePair<string>>>> ToProperties(this IReadOnlyList<Directory> directories) {
			return
				directories
					.ToDictionary(
						d => d.Name,
						d =>
							d.Tags
								.ToDictionary(
									x => x.Name,
									x => x.Description
								).ToTitleValuePair()
					).ToTitleValuePair();
		}
	}
}
