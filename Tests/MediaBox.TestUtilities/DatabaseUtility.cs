using System;
using System.Collections.Generic;
using System.IO;

using LiteDB;

using Moq;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.TestUtilities {
	public static class DatabaseUtility {
		public static ILiteCollection<T> ToLiteDbCollection<T>(this IEnumerable<T> tableData) {
			var db = new LiteDatabase(":memory:");
			var collection = db.GetCollection<T>(typeof(T).Name);
			collection.InsertBulk(tableData);
			return collection;
		}

		public static IMediaFileModel ToModel(this MediaFile mediaFile) {
			var mock = new Mock<IMediaFileModel>();
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
			return mock.Object;
		}

		public static void RegisterMediaFileRecord(
			DocumentDb documentDb,
			string filePath,
			long? mediaFileId = null,
			string thumbnailFileName = null,
			double? latitude = double.NaN,
			double? longitude = double.NaN,
			double? altitude = double.NaN,
			long? fileSize = null,
			int? rate = null,
			int? width = null,
			int? height = null,
			string hash = null,
			string[] tags = null,
			SubTable subTable = SubTable.None,
			int? orientation = null,
			double? duration = null,
			int? rotation = null,
			Position position = null
			) {
			lock (documentDb) {
				var mf = new MediaFile {
					DirectoryPath = $@"{Path.GetDirectoryName(filePath)}\",
					FilePath = filePath,
					MediaFileId = mediaFileId ?? 0,
					ThumbnailFileName = thumbnailFileName ?? "abcdefg"
				};
				if (latitude is { } d && double.IsNaN(d)) {
					mf.Latitude = 35.4218;
				} else {
					mf.Latitude = latitude;
				}
				if (longitude is { } d2 && double.IsNaN(d2)) {
					mf.Longitude = 134.1291;
				} else {
					mf.Longitude = longitude;
				}
				if (altitude is { } d3 && double.IsNaN(d3)) {
					mf.Altitude = 15.291;
				} else {
					mf.Altitude = altitude;
				}
				mf.FileSize = fileSize ?? 210511;
				mf.Rate = rate ?? 0;
				mf.Width = width ?? 640;
				mf.Height = height ?? 480;
				mf.Hash = hash ?? "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b";
				mf.Tags = tags ?? Array.Empty<string>();
				if (subTable.HasFlag(SubTable.Image)) {
					mf.ImageFile = new ImageFile {
						Orientation = orientation ?? 0
					};
				}
				if (subTable.HasFlag(SubTable.Video)) {
					mf.VideoFile = new VideoFile {
						Duration = duration,
						Rotation = rotation
					};
				}
				var collection = documentDb.GetPositionsCollection();
				if (mf.Latitude is { } lat && mf.Longitude is { } lon) {
					if (collection.Query().Where(x => x.Latitude == lat && x.Longitude == lon).FirstOrDefault() == null) {
						if (position == null) {
							collection.Insert(new Position() { Latitude = lat, Longitude = lon });
						} else {
							collection.Insert(position);
						}
					}
				} else if (position != null) {
					collection.Insert(position);
				}

				documentDb.GetMediaFilesCollection().Insert(mf);
			}
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
