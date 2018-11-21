using ExifLib;
using Livet;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Repository;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Threading;
using Unity;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルリストクラス
	/// </summary>
	internal class MediaFileList : ModelBase {
		/// <summary>
		/// メディアファイルリスト
		/// </summary>
		public ReactiveCollection<MediaFile> Items {
			get;
		} = new ReactiveCollection<MediaFile>(UIDispatcherScheduler.Default);

		/// <summary>
		/// キュー
		/// </summary>
		private ReactiveCollection<MediaFile> Queue {
			get;
		} = new ReactiveCollection<MediaFile>();

		public MediaFileList() {
			// キューに入ったメディアを処理しながらメディアファイルリストに移していく
			this.Queue
				.ToCollectionChanged()
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => {
					if (x.Action == NotifyCollectionChangedAction.Add) {
						x.Value.CreateThumbnail();
						this.Queue.Remove(x.Value);
						this.Items.Add(x.Value);
						try {
							var reader = new ExifReader(x.Value.FilePath.Value);
							reader.GetTagValue(ExifTags.GPSLatitudeRef, out string latitudeRef);
							reader.GetTagValue(ExifTags.GPSLongitudeRef, out string longitudeRef);
							reader.GetTagValue(ExifTags.GPSLatitude, out double[] latitude);
							reader.GetTagValue(ExifTags.GPSLongitude, out double[] longitude);

							if (new object[] { latitude, longitude, latitudeRef, longitudeRef }.All(l => l != null)) {
								x.Value.Latitude.Value = (latitude[0] + (latitude[1] / 60) + latitude[2] / 3600) * (latitudeRef == "S" ? -1 : 1);
								x.Value.Longitude.Value = (longitude[0] + (longitude[1] / 60) + longitude[2] / 3600) * (longitudeRef == "W" ? -1 : 1);
							}
						} catch (ExifLibException) {

						}
						var dbmf = new DataBase.Tables.MediaFile() {
							DirectoryPath = Path.GetDirectoryName(x.Value.FilePath.Value),
							FileName = x.Value.FileName.Value,
							ThumbnailFileName = x.Value.ThumbnailFileName.Value,
							Latitude = x.Value.Latitude.Value,
							Longitude = x.Value.Longitude.Value
						};
						this.DataBase.MediaFiles.Add(dbmf);
						this.DataBase.SaveChanges();
						x.Value.MediaFileId = dbmf.MediaFileId;
					}
				});
		}

		/// <summary>
		/// データベースからメディアファイルの読み込み
		/// </summary>
		public void Load() {
			this.Items.AddRangeOnScheduler(
				this.DataBase.MediaFiles.AsEnumerable().Select(x => {
					var m = UnityConfig.UnityContainer.Resolve<MediaFile>().Initialize(Path.Combine(x.DirectoryPath, x.FileName));
					m.ThumbnailFileName.Value = x.ThumbnailFileName;
					m.Latitude.Value = x.Latitude;
					m.Longitude.Value = x.Longitude;
					return m;
				})
			);
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="path">ディレクトリパス</param>
		public void Load(string path) {
			if (!Directory.Exists(path)) {
				return;
			}
			this.Queue.AddRangeOnScheduler(
				Directory
					.EnumerateFiles(path)
					.Where(x => this.Settings.GeneralSettings.TargetExtensions.Contains(Path.GetExtension(x).ToLower()))
					.Where(x => this.Queue.All(m => m.FilePath.Value != x))
					.Where(x => this.DataBase.MediaFiles.All(m => Path.GetFileName(x) != m.FileName || Path.GetDirectoryName(x) != m.DirectoryPath))
					.Select(x => UnityConfig.UnityContainer.Resolve<MediaFile>().Initialize(x))
					.ToList());
		}
	}
}
