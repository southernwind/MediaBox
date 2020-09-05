using System.Collections.Generic;
using System.IO;

using LiteDB;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.TestUtilities {
	public static class DatabaseUtility {
		public static ILiteCollection<T> ToLiteDbCollection<T>(this IEnumerable<T> tableData) {
			var db = new LiteDatabase(":memory:");
			var collection = db.GetCollection<T>(typeof(T).Name);
			collection.InsertBulk(tableData);
			return collection;
		}

		public static IMediaFileModel ToModel(this MediaFile mediaFile) {
			var mock = ModelMockCreator.CreateMediaFileModelMock();
			mock.SetupAllProperties();
			mock.Setup(m => m.MediaFileId).Returns(mediaFile.MediaFileId);
			mock.Setup(m => m.Exists).Returns(File.Exists(mediaFile.FilePath));
			mock.Setup(m => m.FilePath).Returns(mediaFile.FilePath);
			if (mediaFile.Latitude is { } latitude && mediaFile.Longitude is { } longitude) {
				mock.Setup(m => m.Location).Returns(new GpsLocation(latitude, longitude, mediaFile.Altitude));
			} else {
				mock.Setup(m => m.Location).Returns(null as GpsLocation);
			}
			mock.Setup(m => m.Rate).Returns(mediaFile.Rate);
			mock.Setup(m => m.Resolution).Returns(new ComparableSize(mediaFile.Width, mediaFile.Height));
			var tagRc = new ReactiveCollection<string>();
			if (mediaFile.Tags != null) {
				tagRc.AddRange(mediaFile.Tags);
			}
			mock.Setup(m => m.Tags).Returns(tagRc);
			return mock.Object;
		}

	}
	public enum SubTable {
		None,
		Image,
		Video,
		Jpeg,
		Png
	}
}
