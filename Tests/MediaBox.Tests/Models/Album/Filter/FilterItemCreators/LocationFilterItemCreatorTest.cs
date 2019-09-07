using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class LocationFilterItemCreatorTest : FilterCreatorTestClassBase {
		protected override void SetDatabaseRecord() {
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, latitude: null, longitude: null, altitude: null);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, latitude: 38.9156, longitude: -77.0561, altitude: null);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, latitude: null, longitude: null, altitude: null);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image4Png.FilePath, mediaFileId: 4, latitude: 41.305779, longitude: 69.291553, altitude: null);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, latitude: null, longitude: null, altitude: null);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, latitude: null, longitude: null, altitude: null);
		}

		protected override IEnumerable<IMediaFileModel> CreateModels() {
			var m1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			m1.MediaFileId = 1;
			m1.Location = null;
			var m2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			m2.MediaFileId = 2;
			m2.Location = new GpsLocation(38.9156, -77.0561);
			var m3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			m3.MediaFileId = 3;
			m3.Location = null;
			var m4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			m4.MediaFileId = 4;
			m4.Location = new GpsLocation(41.305779, 69.291553);
			var m5 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			m5.MediaFileId = 5;
			m5.Location = null;
			var m6 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);
			m6.MediaFileId = 6;
			m6.Location = null;

			return new[] { m1, m2, m3, m4, m5, m6 };
		}

		[TestCase(true, "座標情報を含む")]
		[TestCase(false, "座標情報を含まない")]
		public void プロパティ(bool contains, string displayName) {
			var ic = new LocationFilterItemCreator(contains);
			ic.Contains.Is(contains);
			ic.DisplayName.Is(displayName);

#pragma warning disable 618
			var ic2 = new LocationFilterItemCreator();
#pragma warning restore 618
			ic2.Contains.IsNull();
			ic2.Contains = contains;
			ic2.Contains.Is(contains);
			ic2.DisplayName.Is(displayName);
		}


		[TestCase(true, 2, 4)]
		[TestCase(false, 1, 3, 5, 6)]
		public void フィルタリングテスト(bool contains, params long[] idList) {
			this.SetDatabaseRecord();
			var ic = new LocationFilterItemCreator(contains);
			var filter = ic.Create() as FilterItem;
			this.DataBase.MediaFiles.Where(filter.Condition).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList);
			this.CreateModels().Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList.Cast<long?>());
		}
	}
}
