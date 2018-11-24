using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Library.EventAsObservable;
using SandBeige.MediaBox.Repository;
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

		/// <summary>
		/// ファイル更新監視
		/// </summary>
		public ReadOnlyReactiveCollection<FileSystemWatcher> FileSystemWatchers {
			get;
			private set;
		}

		public MediaFileList() {
			// キューに入ったメディアを処理しながらメディアファイルリストに移していく
			this.Queue
				.ToCollectionChanged()
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => {
					if (x.Action == NotifyCollectionChangedAction.Add) {
						this.AddItem(x.Value);
						this.Queue.Remove(x.Value);
					}
				});
		}

		/// <summary>
		/// 初期処理
		/// </summary>
		/// <returns>this</returns>
		public MediaFileList Initialize() {
			// ファイル更新監視
			this.FileSystemWatchers = this.Settings
				.PathSettings
				.MonitoringDirectories
				.ToReadOnlyReactiveCollection(md => {
					if (!Directory.Exists(md.DirectoryPath.Value)) {
						this.Logging.Log(LogLevel.Warning, $"監視フォルダが見つかりません。{md.DirectoryPath.Value}");
						return null;
					}
					// 初期読み込み
					this.Load(md.DirectoryPath.Value);
					var fsw = new FileSystemWatcher(md.DirectoryPath.Value) {
						IncludeSubdirectories = true,
						EnableRaisingEvents = md.Monitoring.Value
					};
					var disposable = Observable.Merge(
						fsw.CreatedAsObservable(),
						fsw.RenamedAsObservable(),
						fsw.ChangedAsObservable(),
						fsw.DeletedAsObservable()
						).Subscribe(x => {
							if (!this.Settings.GeneralSettings.TargetExtensions.Value.Contains(Path.GetExtension(x.FullPath))) {
								return;
							}
							if (x.ChangeType == WatcherChangeTypes.Created) {
								this.Queue.AddOnScheduler(UnityConfig.UnityContainer.Resolve<MediaFile>().Initialize(x.FullPath));
							}
						});

					fsw.DisposedAsObservable().Subscribe(_ => disposable.Dispose());

					md.DirectoryPath.Where(Directory.Exists).Subscribe(x => {
						fsw.Path = x;
					});

					md.Monitoring.Subscribe(x => {
						fsw.EnableRaisingEvents = x;
					});

					return fsw;
				});
			return this;
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
					.EnumerateFiles(path,"*",SearchOption.AllDirectories)
					.Where(x => this.Settings.GeneralSettings.TargetExtensions.Value.Contains(Path.GetExtension(x).ToLower()))
					.Where(x => this.Queue.All(m => m.FilePath.Value != x))
					.Where(x => this.DataBase.MediaFiles.All(m => Path.GetFileName(x) != m.FileName || Path.GetDirectoryName(x) != m.DirectoryPath))
					.Select(x => UnityConfig.UnityContainer.Resolve<MediaFile>().Initialize(x))
					.ToList());
		}

		private void AddItem(MediaFile mediaFile) {
			mediaFile.CreateThumbnail();
			mediaFile.LoadExif();
			this.Items.Add(mediaFile);
			var dbmf = new DataBase.Tables.MediaFile() {
				DirectoryPath = Path.GetDirectoryName(mediaFile.FilePath.Value),
				FileName = mediaFile.FileName.Value,
				ThumbnailFileName = mediaFile.ThumbnailFileName.Value,
				Latitude = mediaFile.Latitude.Value,
				Longitude = mediaFile.Longitude.Value
			};
			this.DataBase.MediaFiles.Add(dbmf);
			this.DataBase.SaveChanges();
			mediaFile.MediaFileId = dbmf.MediaFileId;
		}
	}
}
