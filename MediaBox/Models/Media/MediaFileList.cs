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

		public ReactiveCollection<MediaFile> Queue {
			get;
		} = new ReactiveCollection<MediaFile>();

		public MediaFileList() {
			this.Queue
				.ToCollectionChanged()
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => {
					if (x.Action == NotifyCollectionChangedAction.Add) {
						x.Value.CreateThumbnail();
						Thread.Sleep(1000);
						this.Queue.Remove(x.Value);
						this.Items.Add(x.Value);
						var dbmf = new DataBase.Tables.MediaFile() {
							DirectoryPath = Path.GetDirectoryName(x.Value.FilePath.Value),
							FileName = x.Value.FileName.Value,
							ThumbnailFileName = x.Value.ThumbnailFileName.Value
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
