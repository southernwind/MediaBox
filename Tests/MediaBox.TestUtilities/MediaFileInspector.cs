using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.TestUtilities.TestData;

namespace SandBeige.MediaBox.TestUtilities {

	/// <summary>
	/// テストファイル検証クラス
	/// </summary>
	public static class MediaFileInspector {
		/// <summary>
		/// ファイル検証(複数)
		/// </summary>
		/// <param name="mediaFiles">検証対象</param>
		/// <param name="testFiles">想定される結果</param>
		public static void Check(this IEnumerable<IMediaFileModel> mediaFiles, params TestFile[] testFiles) {
			Check(mediaFiles, testFiles.AsEnumerable());
		}

		/// <summary>
		/// ファイル検証(複数)
		/// </summary>
		/// <param name="mediaFiles">検証対象</param>
		/// <param name="testFiles">想定される結果</param>
		/// <param name="includeFileName">検証にファイル名を含むか否か</param>
		/// <param name="includeTime">検証に時刻を含むか否か</param>
		public static void Check(this IEnumerable<IMediaFileModel> mediaFiles, IEnumerable<TestFile> testFiles, bool includeFileName = true, bool includeTime = true) {
			mediaFiles.Count().Is(testFiles.Count());
			var mfs = mediaFiles.OrderBy(m => m.FilePath).ToArray();
			var tfs = testFiles.OrderBy(m => m.FilePath).ToArray();
			foreach (var i in mfs.Select((x, i) => i)) {
				Check(mfs[i], tfs[i], includeFileName, includeTime);
			}
		}

		/// <summary>
		/// ファイル検証(単体)
		/// </summary>
		/// <param name="media">検証対象</param>
		/// <param name="test">想定される結果</param>
		/// <param name="includeFileName">検証にファイル名を含むか否か</param>
		/// <param name="includeTime">検証に時刻を含むか否か</param>
		public static void Check(this IMediaFileModel media, TestFile test, bool includeFileName = true, bool includeTime = true) {
			if (includeFileName) {
				media.FileName.Is(test.FileName);
				media.FilePath.Is(test.FilePath);
				media.Extension.Is(test.Extension);
			}
			if (includeTime) {
				media.CreationTime.Is(test.CreationTime);
				media.ModifiedTime.Is(test.ModifiedTime);
				media.LastAccessTime.Is(test.LastAccessTime);
			}
			media.FileSize.Is(test.FileSize);
			media.Resolution.Is(test.Resolution);
			OriginalAssert.AreEqual(test.Location?.Latitude, media.Location?.Latitude, 0.01);
			OriginalAssert.AreEqual(test.Location?.Longitude, media.Location?.Longitude, 0.01);
			OriginalAssert.AreEqual(test.Location?.Altitude, media.Location?.Altitude, 0.01);
			media.Rate.Is(test.Rate);
			media.IsInvalid.Is(test.IsInvalid);
			media.Tags.Is(test.Tags);
			media.Exists.Is(test.Exists);
		}

		/// <summary>
		/// ファイル検証(複数)
		/// </summary>
		/// <param name="mediaFiles">検証対象</param>
		/// <param name="testFiles">想定される結果</param>
		public static void Check(this IQueryable<MediaFile> mediaFiles, params TestFile[] testFiles) {
			Check(mediaFiles, testFiles.AsEnumerable());
		}

		/// <summary>
		/// ファイル検証(複数)
		/// </summary>
		/// <param name="mediaFiles">検証対象</param>
		/// <param name="testFiles">想定される結果</param>
		/// <param name="includeFileName">検証にファイル名を含むか否か</param>
		public static void Check(this IQueryable<MediaFile> mediaFiles, IEnumerable<TestFile> testFiles, bool includeFileName = true) {
			var records =
				mediaFiles
					.Include(mf => mf.MediaFileTags)
					.ThenInclude(mft => mft.Tag)
					.Include(mf => mf.ImageFile)
					.Include(mf => mf.VideoFile)
					.ToArray();
			records.Count().Is(testFiles.Count());
			var mfs = records.OrderBy(m => m.FilePath).ToArray();
			var tfs = testFiles.OrderBy(m => m.FilePath).ToArray();
			foreach (var i in mfs.Select((x, i) => i)) {
				Check(mfs[i], tfs[i], includeFileName);
			}
		}

		/// <summary>
		/// ファイル検証(単体)
		/// </summary>
		/// <param name="media">検証対象</param>
		/// <param name="test">想定される結果</param>
		/// <param name="includeFileName">検証にファイル名を含むか否か</param>
		public static void Check(this MediaFile media, TestFile test, bool includeFileName = true) {
			if (includeFileName) {
				media.FilePath.Is(test.FilePath);
				media.FileSize.Is(test.FileSize);
			}
			OriginalAssert.AreEqual(test.Location?.Latitude, media.Latitude, 0.01);
			OriginalAssert.AreEqual(test.Location?.Longitude, media.Longitude, 0.01);
			OriginalAssert.AreEqual(test.Location?.Altitude, media.Altitude, 0.01);
			media.Rate.Is(test.Rate);
			media.MediaFileTags.Select(x => x.Tag.TagName).Is(test.Tags);
		}
	}
}
