using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.TestUtilities.TestData;

namespace SandBeige.MediaBox.TestUtilities {


	public static class MediaFileInspector {
		public static void Check(this IEnumerable<IMediaFileModel> mediaFiles, IEnumerable<TestFile> testFiles) {
			mediaFiles.Count().Is(testFiles.Count());
			var mfs = mediaFiles.OrderBy(m => m.FilePath).ToArray();
			var tfs = testFiles.OrderBy(m => m.FilePath).ToArray();
			foreach (var i in mfs.Select((x, i) => i)) {
				Check(mfs[i], tfs[i]);
			}
		}

		public static void Check(this IMediaFileModel media, TestFile test) {
			media.FileName.Is(test.FileName);
			media.FilePath.Is(test.FilePath);
			media.Extension.Is(test.Extension);
			media.CreationTime.Is(test.CreationTime);
			media.ModifiedTime.Is(test.ModifiedTime);
			media.LastAccessTime.Is(test.LastAccessTime);
			media.FileSize.Is(test.FileSize);
			media.Resolution.Is(test.Resolution);
			media.Location.Is(test.Location);
			media.Rate.Is(test.Rate);
			media.IsInvalid.Is(test.IsInvalid);
			media.Tags.Is(test.Tags);
			media.Exists.Is(test.Exists);
		}
	}
}
