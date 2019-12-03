using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class MediaTypeFilterItemCreatorTest : FilterCreatorTestClassBase {
		protected override void SetDatabaseRecord() {
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image4Png.FilePath, mediaFileId: 4);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Video1Mov.FilePath, mediaFileId: 6);
		}

		protected override IEnumerable<IMediaFileModel> CreateModels() {
			var m1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			m1.MediaFileId = 1;
			var m2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			m2.MediaFileId = 2;
			var m3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			m3.MediaFileId = 3;
			var m4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			m4.MediaFileId = 4;
			var m5 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			m5.MediaFileId = 5;
			var m6 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);
			m6.MediaFileId = 6;

			return new[] { m1, m2, m3, m4, m5, m6 };
		}

		[TestCase(true, "動画ファイル")]
		[TestCase(false, "画像ファイル")]
		public void プロパティ(bool isVideo, string displayName) {
			var ic = new MediaTypeFilterItemCreator(isVideo);
			ic.IsVideo.Is(isVideo);
			ic.DisplayName.Is(displayName);

#pragma warning disable 618
			var ic2 = new MediaTypeFilterItemCreator();
#pragma warning restore 618
			ic2.IsVideo.Is(false);
			ic2.IsVideo = isVideo;
			ic2.IsVideo.Is(isVideo);
			ic2.DisplayName.Is(displayName);
		}


		[TestCase(true, 6)]
		[TestCase(false, 1, 2, 3, 4, 5)]
		public void フィルタリングテスト(bool isVideo, params long[] idList) {
			this.SetDatabaseRecord();
			var ic = new MediaTypeFilterItemCreator(isVideo);
			var filter = ic.Create() as FilterItem;
			this.DocumentDb.GetMediaFilesCollection().Query().Where(filter.Condition).Select(x => x.MediaFileId).ToEnumerable().OrderBy(x => x).Is(idList);
			this.CreateModels().Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList.Cast<long?>());
		}
	}
}
