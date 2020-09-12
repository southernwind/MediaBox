using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class LocationFilterItemCreatorTest : FilterCreatorTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.TestTableData = new[] {
				new MediaFile { MediaFileId = 1, Latitude= null, Longitude= null, Altitude= null },
				new MediaFile{ MediaFileId= 2, Latitude= 38.9156, Longitude= -77.0561,Altitude= null},
				new MediaFile{ MediaFileId= 3, Latitude= null, Longitude= null, Altitude= null},
				new MediaFile{ MediaFileId= 4, Latitude= 41.305779, Longitude= 69.291553, Altitude= null},
				new MediaFile{ MediaFileId= 5, Latitude= null, Longitude= null, Altitude= null},
				new MediaFile{ MediaFileId= 6, Latitude= null, Longitude= null, Altitude= null}
			};
			this.CreateModels();
		}

		[TestCase(true, 2, 4)]
		[TestCase(false, 1, 3, 5, 6)]
		public void フィルタリングテスト(bool contains, params long[] idList) {
			var io = new LocationFilterItemObject(contains);
			var ic = new LocationFilterItemCreator();
			var filter = ic.Create(io);
			filter.IncludeSql.Should().Be(true);
			this.TestTableData!.ToLiteDbCollection().Query().Where(filter.Condition).Select(x => x.MediaFileId).ToEnumerable().OrderBy(x => x).Should().Equal(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(idList.Cast<long?>());
		}
	}
}
