using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class LocationFilterItemCreatorTest : FilterCreatorTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.TestTableData = new[] {
				DatabaseUtility.CreateMediaFileRecord(1, latitude: null, longitude: null, altitude: null),
				DatabaseUtility.CreateMediaFileRecord(2, latitude: 38.9156, longitude: -77.0561,altitude: null),
				DatabaseUtility.CreateMediaFileRecord(3, latitude: null, longitude: null, altitude: null),
				DatabaseUtility.CreateMediaFileRecord(4, latitude: 41.305779, longitude: 69.291553, altitude: null),
				DatabaseUtility.CreateMediaFileRecord(5, latitude: null, longitude: null, altitude: null),
				DatabaseUtility.CreateMediaFileRecord(6, latitude: null, longitude: null, altitude: null)
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
			this.TestTableData!.AsQueryable().Where(filter.Condition).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(idList.Cast<long?>());
		}
	}
}
