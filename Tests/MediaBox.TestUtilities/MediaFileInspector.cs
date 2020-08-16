using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using LiteDB;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.DataBase.Tables.Metadata;
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
			mediaFiles.Count().Should().Be(testFiles.Count());
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
				media.FileName.Should().Be(test.FileName);
				media.FilePath.Should().Be(test.FilePath);
				media.Extension.Should().Be(test.Extension);
			}
			if (includeTime) {
				media.CreationTime.Should().Be(test.CreationTime);
				media.ModifiedTime.Should().Be(test.ModifiedTime);
				media.LastAccessTime.Should().Be(test.LastAccessTime);
			}
			media.FileSize.Should().Be(test.FileSize);
			media.Resolution.Should().Be(test.Resolution);
			OriginalAssert.AreEqual(test.Location?.Latitude, media.Location?.Latitude, 0.01);
			OriginalAssert.AreEqual(test.Location?.Longitude, media.Location?.Longitude, 0.01);
			OriginalAssert.AreEqual(test.Location?.Altitude, media.Location?.Altitude, 0.01);
			media.Rate.Should().Be(test.Rate);
			media.IsInvalid.Should().Be(test.IsInvalid);
			media.Tags.Should().Equal(test.Tags);
			media.Exists.Should().Be(test.Exists);
		}

		/// <summary>
		/// ファイル検証(複数)
		/// </summary>
		/// <param name="mediaFiles">検証対象</param>
		/// <param name="testFiles">想定される結果</param>
		public static void Check(this ILiteQueryable<MediaFile> mediaFiles, params TestFile[] testFiles) {
			Check(mediaFiles, testFiles.AsEnumerable());
		}

		/// <summary>
		/// ファイル検証(複数)
		/// </summary>
		/// <param name="mediaFiles">検証対象</param>
		/// <param name="testFiles">想定される結果</param>
		/// <param name="includeFileName">検証にファイル名を含むか否か</param>
		public static void Check(this ILiteQueryable<MediaFile> mediaFiles, IEnumerable<TestFile> testFiles, bool includeFileName = true) {
			var records =
				mediaFiles
					.Include(x => x.Position)
					.ToArray();
			records.Count().Should().Be(testFiles.Count());
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
				media.FilePath.Should().Be(test.FilePath);
				media.FileSize.Should().Be(test.FileSize);
			}
			OriginalAssert.AreEqual(test.Location?.Latitude, media.Latitude, 0.01);
			OriginalAssert.AreEqual(test.Location?.Longitude, media.Longitude, 0.01);
			OriginalAssert.AreEqual(test.Location?.Altitude, media.Altitude, 0.01);
			media.Rate.Should().Be(test.Rate);
			media.Tags.Should().Equal(test.Tags);
			if (test.Jpeg == null) {
				media.Jpeg.Should().BeNull();
			} else {
				media.Jpeg.Should().NotBeNull();
				CheckAllProperties(test.Jpeg, media.Jpeg);
			}

			if (test.Png == null) {
				media.Png.Should().BeNull();
			} else {
				media.Png.Should().NotBeNull();
				CheckAllProperties(test.Png, media.Png);
			}

			if (test.Bmp == null) {
				media.Bmp.Should().BeNull();
			} else {
				media.Bmp.Should().NotBeNull();
				CheckAllProperties(test.Bmp, media.Bmp);
			}

			if (test.Gif == null) {
				media.Gif.Should().BeNull();
			} else {
				media.Gif.Should().NotBeNull();
				CheckAllProperties(test.Gif, media.Gif);
			}
		}

		private static void CheckAllProperties<T>(T expect, T actual) {
			foreach (var property in typeof(T).GetProperties()) {
				if (
					property.Name == nameof(Jpeg.MediaFileId) ||
					property.Name == nameof(Jpeg.MediaFile)) {
					continue;
				}
				var expectValue = property.GetValue(expect);
				var actualValue = property.GetValue(actual);
				actualValue.Should().Be(expectValue, $"{property.Name}");
			}
		}
	}
}
