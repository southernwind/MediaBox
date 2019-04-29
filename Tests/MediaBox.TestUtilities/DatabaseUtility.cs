﻿using System.IO;

using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.TestUtilities {
	public static class DatabaseUtility {
		public static MediaFile GetMediaFileRecord(
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
			string hash = null) {
			var mf = new MediaFile {
				DirectoryPath = $@"{Path.GetDirectoryName(filePath)}\",
				FilePath = filePath,
				MediaFileId = mediaFileId ?? 0,
				ThumbnailFileName = thumbnailFileName ?? "abcdefg"
			};
			if (latitude is {} d && double.IsNaN(d)) {
				mf.Latitude = 35.4218;
			} else {
				mf.Latitude = latitude;
			}
			if (longitude is {} d2 && double.IsNaN(d2)) {
				mf.Longitude = 134.1291;
			} else {
				mf.Longitude = longitude;
			}
			if (altitude is {} d3 && double.IsNaN(d3)) {
				mf.Altitude = 15.291;
			} else {
				mf.Altitude = altitude;
			}
			mf.FileSize = fileSize ?? 210511;
			mf.Rate = rate ?? 0;
			mf.Width = width ?? 640;
			mf.Height = height ?? 480;
			mf.Hash = hash ?? "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b";
			return mf;
		}
	}
}
