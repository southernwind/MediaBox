using System;
using System.IO;
using System.Linq;

using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.TestUtilities {
	public static class DatabaseUtility {
		public static void RegisterMediaFileRecord(
			MediaBoxDbContext dbContext,
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
			mf.MediaFileTags = (tags ?? Array.Empty<string>()).Select(x => new MediaFileTag() { Tag = new Tag() { TagName = x } }).ToArray();
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
			if (mf.Latitude is { } lat && mf.Longitude is { } lon) {
				if (!dbContext.Positions.Any(x => x.Latitude == lat && x.Longitude == lon)) {
					if (position == null) {
						dbContext.Positions.Add(new Position() { Latitude = lat, Longitude = lon });
					} else {
						dbContext.Positions.Add(position);
					}
				}
			} else if (position != null) {
				dbContext.Positions.Add(position);
			}
			dbContext.MediaFiles.Add(mf);
			dbContext.SaveChanges();
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
