﻿using Livet;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ExifLib;

namespace SandBeige.MediaBox.Models.Media
{
	/// <summary>
	/// メディアファイルクラス
	/// </summary>
    class MediaFile:ModelBase {
		public long? MediaFileId {
			get;
			set;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public ReadOnlyReactiveProperty<string> FileName {
			get;
			private set;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public ReactivePropertySlim<string> FilePath {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// サムネイルファイルパス
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> ThumbnailFilePath {
			get;
			private set;
		}

		/// <summary>
		/// サムネイルファイル名
		/// </summary>
		public ReactivePropertySlim<string> ThumbnailFileName {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 緯度
		/// </summary>
		public ReactivePropertySlim<double> Latitude {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// 経度
		/// </summary>
		public ReactivePropertySlim<double> Longitude {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// 初期処理
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <returns><see cref="this"/></returns>
		public MediaFile Initialize(string filePath) {
			this.FilePath.Value = filePath;
			this.FileName = this.FilePath.Select(x => Path.GetFileName(x)).ToReadOnlyReactiveProperty();
			this.ThumbnailFilePath = this.ThumbnailFileName.Where(x => x != null).Select(x => Path.Combine(this.Settings.GeneralSettings.ThumbnailDirectoryPath, x)).ToReadOnlyReactivePropertySlim();
			try {
				var reader = new ExifReader(this.FilePath.Value);
				reader.GetTagValue(ExifTags.GPSLatitudeRef, out string latitudeRef);
				reader.GetTagValue(ExifTags.GPSLongitudeRef, out string longitudeRef);
				reader.GetTagValue(ExifTags.GPSLatitude, out double[] latitude);
				reader.GetTagValue(ExifTags.GPSLongitude, out double[] longitude);

				this.Latitude.Value = (latitude[0] + (latitude[1] / 60) + latitude[2] / 3600) * (latitudeRef == "S" ? -1 : 1);
				this.Longitude.Value = (longitude[0] + (longitude[1] / 60) + longitude[2] / 3600) * (longitudeRef == "W" ? -1 : 1);
			} catch (ExifLibException) {

			}
			return this;
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		public void CreateThumbnail() {
			using (var crypto = new SHA256CryptoServiceProvider()) {
				var file = File.ReadAllBytes(this.FilePath.Value);

				var thumbnail = ThumbnailCreator.Create(file, 200, 200).ToArray();
				var thumbFileName = $"{string.Join("", crypto.ComputeHash(thumbnail).Select(b => $"{b:X2}"))}.jpg";
				var thumbFilePath = Path.Combine(this.Settings.GeneralSettings.ThumbnailDirectoryPath, thumbFileName);
				if (!File.Exists(thumbFilePath)) {
					File.WriteAllBytes(thumbFilePath, thumbnail);
				}
				this.ThumbnailFileName.Value = thumbFileName;
			}
		}
	}
}
