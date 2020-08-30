using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.DataBase.Tables.Metadata;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Services;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイル情報
	/// </summary>
	/// <remarks>
	/// 複数のメディアファイルの情報をまとめて閲覧できるようにする
	/// </remarks>
	public class MediaFileInformation : ModelBase, IMediaFileInformation {
		private readonly IPriorityTaskQueue _priorityTaskQueue;
		private readonly IDocumentDb _documentDb;
		private readonly IMediaBoxDbContext _rdb;
		private readonly ILogging _logging;
		private readonly IGeoCodingManager _geoCodingManager;
		private readonly IMediaFileManager _mediaFileManager;

		/// <summary>
		/// タグリスト
		/// </summary>
		public IReactiveProperty<IEnumerable<ValueCountPair<string>>> Tags {
			get;
		} = new ReactivePropertySlim<IEnumerable<ValueCountPair<string>>>();

		/// <summary>
		/// ファイルリスト
		/// </summary>
		public IReadOnlyReactiveProperty<IEnumerable<IMediaFileModel>> Files {
			get;
		}

		/// <summary>
		/// ファイル数
		/// </summary>
		public IReadOnlyReactiveProperty<int> FilesCount {
			get;
		}

		/// <summary>
		/// 更新中
		/// </summary>
		public IReactiveProperty<bool> Updating {
			get;
		} = new ReactivePropertySlim<bool>();

		/// <summary>
		/// 代表値
		/// </summary>
		public IReadOnlyReactiveProperty<IMediaFileModel> RepresentativeMediaFile {
			get;
		}

		/// <summary>
		/// プロパティ
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileProperty>> Properties {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileProperty>>();

		/// <summary>
		/// メタデータ
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileProperty>> Metadata {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileProperty>>();

		/// <summary>
		/// GPS座標
		/// </summary>
		public IReactiveProperty<IAddress> Positions {
			get;
		} = new ReactivePropertySlim<IAddress>();

		/// <summary>
		/// 評価平均
		/// </summary>
		public IReactiveProperty<double> AverageRate {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileInformation(IDocumentDb documentDb, IMediaBoxDbContext rdb, ILogging logging, IPriorityTaskQueue priorityTaskQueue, IGeoCodingManager geoCodingManager, IMediaFileManager mediaFileManager, VolatilityStateShareService volatilityStateShareService) {
			this._documentDb = documentDb;
			this._rdb = rdb;
			this._logging = logging;
			this._priorityTaskQueue = priorityTaskQueue;
			this._geoCodingManager = geoCodingManager;
			this._mediaFileManager = mediaFileManager;
			this.Files = volatilityStateShareService.MediaFileModels.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.FilesCount = this.Files.Select(x => x.Count()).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.RepresentativeMediaFile = this.Files.Select(Enumerable.FirstOrDefault).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Files
				.ObserveOn(TaskPoolScheduler.Default)
				.Do(x => {
					this.Updating.Value = true;
					this.Tags.Value = Array.Empty<ValueCountPair<string>>();
					this.Properties.Value = Array.Empty<IMediaFileProperty>();
					this.Positions.Value = null;
					this.Metadata.Value = Array.Empty<IMediaFileProperty>();
					this.AverageRate.Value = double.NaN;
				})
				.Throttle(TimeSpan.FromMilliseconds(100))
				.Synchronize()
				.Subscribe(x => {
					using (this.DisposeLock.DisposableEnterReadLock()) {
						if (this.DisposeState != DisposeState.NotDisposed) {
							return;
						}
						this.UpdateTags();
						this.UpdateProperties();
						this.UpdateMetadata();
						this.UpdateRate();
						this.Updating.Value = false;
					}
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// 対象ファイルすべての評価設定
		/// </summary>
		/// <param name="rate"></param>
		public void SetRate(int rate) {
			lock (this._rdb) {
				var targetArray = this.Files.Value;
				this._documentDb
					.GetMediaFilesCollection()
					.UpdateMany(
						x => new MediaFile { Rate = rate },
						x => targetArray.Select(m => m.MediaFileId.Value).Contains(x.MediaFileId));

				foreach (var item in targetArray) {
					item.Rate = rate;
				}
			}
			this.UpdateRate();
		}

		/// <summary>
		/// 対象ファイルすべてにタグ追加
		/// </summary>
		/// <param name="tagName">追加するタグ名</param>
		public void AddTag(string tagName) {
			var targetArray = this.Files.Value.Where(x => x.MediaFileId.HasValue && !x.Tags.Contains(tagName)).ToArray();

			if (!targetArray.Any()) {
				return;
			}
			lock (this._rdb) {
				var col = this._documentDb.GetMediaFilesCollection();
				var ids = targetArray.Select(m => m.MediaFileId.Value);
				var list =
					col
						.Query()
						.Where(x => ids.Contains(x.MediaFileId))
						.ToArray();

				foreach (var mf in list) {
					mf.Tags = mf.Tags.Union(new[] { tagName }).ToArray();
					col.Update(mf);
				}


				foreach (var item in targetArray) {
					item.AddTag(tagName);
				}
			}
			this.UpdateTags();
		}

		/// <summary>
		/// 対象ファイルすべてからタグ削除
		/// </summary>
		/// <param name="tagName">削除するタグ名</param>
		public void RemoveTag(string tagName) {
			var targetArray = this.Files.Value.Where(x => x.MediaFileId.HasValue && x.Tags.Contains(tagName)).ToArray();

			if (!targetArray.Any()) {
				return;
			}

			lock (this._rdb) {
				var col = this._documentDb.GetMediaFilesCollection();
				var ids = targetArray.Select(m => m.MediaFileId.Value);
				var list =
					col
						.Query()
						.Where(x => ids.Contains(x.MediaFileId))
						.ToArray();

				foreach (var mf in list) {
					mf.Tags = mf.Tags.Except(new[] { tagName }).ToArray();
					col.Update(mf);
				}

				foreach (var item in targetArray) {
					item.RemoveTag(tagName);
				}
			}
			this.UpdateTags();
		}

		/// <summary>
		/// リバースジオコーディング
		/// </summary>
		/// <remarks>
		/// このクラスで扱っているファイルリストのうち、座標情報を持っているファイルに対してリバースジオコーディングを行う。
		/// キューに追加するだけなので、このメソッドを抜けた時点ではまだ実行されていない。そのうち完了する。
		/// </remarks>
		public void ReverseGeoCoding() {
			foreach (var m in this.Files.Value.Where(x => x.Location != null)) {
				this._geoCodingManager.Reverse(m.Location);
			}
		}

		/// <summary>
		/// サムネイルの作成
		/// </summary>
		public void CreateThumbnail() {
			// タスクはここで発生させる。ただしこのインスタンスが破棄されても動き続ける。
			var files = this.Files.Value.ToArray();
			this._priorityTaskQueue.AddTask(new TaskAction("サムネイル作成", x => Task.Run(() => {
				x.ProgressMax.Value = files.Length;
				foreach (var item in files) {
					if (x.CancellationToken.IsCancellationRequested) {
						return;
					}
					item.CreateThumbnail();
					x.ProgressValue.Value++;
				}
			}), Priority.CreateThumbnail, new CancellationTokenSource()));
		}

		/// <summary>
		/// 指定ディレクトリオープン
		/// </summary>
		/// <param name="filePath"></param>
		public void OpenDirectory(string filePath) {
			try {
				Process.Start("explorer.exe", $"/select,\"{filePath}\"");
			} catch (Exception ex) {
				this._logging.Log($"ディレクトリオープンに失敗しました。[{filePath}]", LogLevel.Error, ex);
			}
		}

		/// <summary>
		/// 登録から削除
		/// </summary>
		public void DeleteFileFromRegistry() {
			this._mediaFileManager.DeleteItems(this.Files.Value);
		}

		/// <summary>
		/// タグ更新
		/// </summary>
		private void UpdateTags() {
			this.Tags.Value =
				this.Files
					.Value
					.SelectMany(x => x.Tags)
					.GroupBy(x => x)
					.Select(x => new ValueCountPair<string>(x.Key, x.Count()));
		}

		/// <summary>
		/// プロパティ更新
		/// </summary>
		private void UpdateProperties() {
			this.Properties.Value =
				this.Files
					.Value
					.SelectMany(x => x.Properties)
					.GroupBy(x => x.Title)
					.Select(x => new MediaFileProperty(
						x.Key,
						x.GroupBy(g => g.Value).Select(g => new ValueCountPair<string>(g.Key, g.Count()))
					));

		}

		/// <summary>
		/// メタデータ更新
		/// </summary>
		private void UpdateMetadata() {
			var ids = this.Files.Value.Select(x => x.MediaFileId).ToArray();

			// TODO : 使用中ライブラリアップデートまでの逃げ
			if (ids.Length == 0) {
				ids = new long?[] { null };
			}

			List<Jpeg> jpegs;
			List<Png> pngs;
			List<Bmp> bmps;
			List<Gif> gifs;
			List<ICollection<VideoMetadataValue>> videoMetadata;

			lock (this._rdb) {
				var mediaFilesCollection = this._documentDb.GetMediaFilesCollection();
				jpegs = mediaFilesCollection
					.Query()
					.Where(x => x.Jpeg != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Select(x => x.Jpeg)
					.ToList()!;
				pngs = mediaFilesCollection
					.Query()
					.Where(x => x.Png != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Select(x => x.Png)
					.ToList()!;
				bmps = mediaFilesCollection
					.Query()
					.Where(x => x.Bmp != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Select(x => x.Bmp)
					.ToList()!;
				gifs = mediaFilesCollection
					.Query()
					.Where(x => x.Gif != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Select(x => x.Gif)
					.ToList()!;
				videoMetadata = mediaFilesCollection
					.Query()
					.Where(x => x.VideoFile != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Select(x => x.VideoFile!.VideoMetadataValues)
					.ToList()!;

				var positions = mediaFilesCollection
					.Query()
					.Where(x => ids.Contains(x.MediaFileId))
					.Where(x => x.Position != null)
					.Select(x => x.Position)
					.ToList();

				this.Positions.Value = new Address(positions!);
			}

			this.Metadata.Value =
				typeof(Jpeg)
					.GetProperties()
					.Select(p =>
						new MediaFileProperty(
							p.Name,
							jpegs
								.Select(x => p.GetValue(x)?.ToString())
								.GroupBy(x => x)
								.Select(x => new ValueCountPair<string>(x.Key, x.Count()))
						)
					).Union(
						typeof(Png)
							.GetProperties()
							.Select(p =>
								new MediaFileProperty(
									p.Name,
									pngs
										.Select(x => p.GetValue(x)?.ToString())
										.GroupBy(x => x)
										.Select(x => new ValueCountPair<string>(x.Key, x.Count()))
								)
							)
					).Union(
						typeof(Bmp)
							.GetProperties()
							.Select(p =>
								new MediaFileProperty(
									p.Name,
									bmps
										.Select(x => p.GetValue(x)?.ToString())
										.GroupBy(x => x)
										.Select(x => new ValueCountPair<string>(x.Key, x.Count()))
								)
							)
					).Union(
						typeof(Gif)
							.GetProperties()
							.Select(p =>
								new MediaFileProperty(
									p.Name,
									gifs
										.Select(x => p.GetValue(x)?.ToString())
										.GroupBy(x => x)
										.Select(x => new ValueCountPair<string>(x.Key, x.Count()))
								)
							)
					).Union(
						videoMetadata
							.SelectMany(x => x)
							.GroupBy(x => x.Key)
							.Select(g => new MediaFileProperty(
								g.Key,
								g.GroupBy(x => x.Value).Select(x => new ValueCountPair<string>(x.Key, x.Count()))))
					).Where(x => x.Values.Any(v => v.Value != null));
		}

		/// <summary>
		/// 評価の更新
		/// </summary>
		private void UpdateRate() {
			var list = this.Files
					.Value
					.Where(x => x.Rate != 0)
					.Select(x => x.Rate)
					.ToArray();
			if (!list.Any()) {
				this.AverageRate.Value = double.NaN;
				return;
			}
			this.AverageRate.Value = list.Average();
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.RepresentativeMediaFile.Value.FilePath} ({this.FilesCount.Value})>";
		}
	}
}
