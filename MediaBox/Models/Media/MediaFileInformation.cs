using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.DataBase.Tables.Metadata;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;
namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイル情報
	/// </summary>
	/// <remarks>
	/// 複数のメディアファイルの情報をまとめて閲覧できるようにする
	/// </remarks>
	internal class MediaFileInformation : ModelBase {
		private readonly IAlbumSelector _selector;
		private readonly PriorityTaskQueue _priorityTaskQueue;

		/// <summary>
		/// タグリスト
		/// </summary>
		public IReactiveProperty<IEnumerable<ValueCountPair<string>>> Tags {
			get;
		} = new ReactivePropertySlim<IEnumerable<ValueCountPair<string>>>();

		/// <summary>
		/// ファイルリスト
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileModel>> Files {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>(Array.Empty<IMediaFileModel>());

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
		public IReactiveProperty<IEnumerable<MediaFileProperty>> Properties {
			get;
		} = new ReactivePropertySlim<IEnumerable<MediaFileProperty>>();

		/// <summary>
		/// メタデータ
		/// </summary>
		public IReactiveProperty<IEnumerable<MediaFileProperty>> Metadata {
			get;
		} = new ReactivePropertySlim<IEnumerable<MediaFileProperty>>();

		/// <summary>
		/// GPS座標
		/// </summary>
		public IReactiveProperty<Address> Positions {
			get;
		} = new ReactivePropertySlim<Address>();

		/// <summary>
		/// 評価平均
		/// </summary>
		public IReactiveProperty<double> AverageRate {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="selector">このメディアファイル情報を保有しているアルバムセレクター</param>
		public MediaFileInformation(IAlbumSelector selector) {
			this._selector = selector;
			this._priorityTaskQueue = Get.Instance<PriorityTaskQueue>();
			this.FilesCount = this.Files.Select(x => x.Count()).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.RepresentativeMediaFile = this.Files.Select(Enumerable.FirstOrDefault).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Files
				.ObserveOn(TaskPoolScheduler.Default)
				.Do(x => {
					this.Updating.Value = true;
					this.Tags.Value = Array.Empty<ValueCountPair<string>>();
					this.Properties.Value = Array.Empty<MediaFileProperty>();
					this.Positions.Value = null;
					this.Metadata.Value = Array.Empty<MediaFileProperty>();
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
			lock (this.DataBase) {
				var targetArray = this.Files.Value;
				this.FilesDataBase
					.GetMediaFilesCollection()
					.UpdateMany(
						x => new MediaFile { Rate = rate },
						x=>targetArray.Select(m => m.MediaFileId.Value).Contains(x.MediaFileId));
				
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

			lock (this.DataBase) {
				this.FilesDataBase
					.GetMediaFilesCollection()
					.UpdateMany(
						x => new MediaFile { Tags = x.Tags.Union(new[] { tagName }).ToArray() },
						x => targetArray.Select(m => m.MediaFileId.Value).Contains(x.MediaFileId));

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

			lock (this.DataBase) {
				this.FilesDataBase
					.GetMediaFilesCollection()
					.UpdateMany(
						x => new MediaFile { Tags = x.Tags.Except(new[] { tagName }).ToArray() },
						x => targetArray.Select(m => m.MediaFileId.Value).Contains(x.MediaFileId));


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
			var gcm = Get.Instance<GeoCodingManager>();
			foreach (var m in this.Files.Value.Where(x => x.Location != null)) {
				gcm.Reverse(m.Location);
			}
		}

		/// <summary>
		/// サムネイルの作成
		/// </summary>
		public void CreateThumbnail() {
			// タスクはここで発生させる。ただしこのインスタンスが破棄されても動き続ける。
			var files = this.Files.Value.ToArray();
			this._priorityTaskQueue.AddTask(new TaskAction("サムネイル作成", async x => {
				x.ProgressMax.Value = files.Length;
				foreach (var item in files) {
					if (x.CancellationToken.IsCancellationRequested) {
						return;
					}
					item.CreateThumbnail();
					x.ProgressValue.Value++;
				}
			}, Priority.CreateThumbnail, new CancellationTokenSource()));
		}

		/// <summary>
		/// 指定ディレクトリオープン
		/// </summary>
		/// <param name="filePath"></param>
		public void OpenDirectory(string filePath) {
			try {
				Process.Start("explorer.exe", $"/select,\"{filePath}\"");
			} catch (Exception ex) {
				this.Logging.Log($"ディレクトリオープンに失敗しました。[{filePath}]", LogLevel.Error, ex);
			}
		}

		/// <summary>
		/// 登録から削除
		/// </summary>
		public void DeleteFileFromRegistry() {
			Get.Instance<MediaFileManager>().DeleteItems(this.Files.Value);
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

			List<Jpeg> jpegs;
			List<Png> pngs;
			List<Bmp> bmps;
			List<Gif> gifs;
			List<ICollection<VideoMetadataValue>> videoMetadata;

			var mediaFilesCollection = this.FilesDataBase.GetMediaFilesCollection();
			lock (this.DataBase) {
				jpegs = mediaFilesCollection
					.Query()
					.Where(x => x.Jpeg != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Select(x => x.Jpeg)
					.ToList();
				pngs = mediaFilesCollection
					.Query()
					.Where(x => x.Png != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Select(x => x.Png)
					.ToList();
				bmps = mediaFilesCollection
					.Query()
					.Where(x => x.Bmp != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Select(x => x.Bmp)
					.ToList();
				gifs = mediaFilesCollection
					.Query()
					.Where(x => x.Gif != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Select(x => x.Gif)
					.ToList();
				videoMetadata = mediaFilesCollection
					.Query()
					.Where(x => x.VideoFile != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Select(x => x.VideoFile.VideoMetadataValues)
					.ToList();

				var positions = mediaFilesCollection
					.Query()
					.Where(x => ids.Contains(x.MediaFileId))
					.Where(x => x.Position != null)
					.Select(x => x.Position)
					.ToList();

				this.Positions.Value = new Address(positions);
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

	/// <summary>
	/// 座標と逆ジオコーディング結果
	/// </summary>
	internal class PositionProperty {
		/// <summary>
		/// 場所名
		/// </summary>
		public string Name {
			get;
		}

		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				if (this.Name != null) {
					return $"{this.Name}[{this.Locations.Count()}]";
				}

				return this.Locations.FirstOrDefault()?.ToString();
			}
		}

		/// <summary>
		/// 同一表示名の座標リスト
		/// </summary>
		public IEnumerable<GpsLocation> Locations {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name">場所名</param>
		/// <param name="locations">同一表示名の座標リスト</param>
		public PositionProperty(string name, IEnumerable<GpsLocation> locations) {
			this.Name = name;
			this.Locations = locations;
		}
	}

	/// <summary>
	/// メディアファイルプロパティ
	/// </summary>
	internal class MediaFileProperty {
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title {
			get;
		}

		/// <summary>
		/// 代表値と件数
		/// </summary>
		public ValueCountPair<string> RepresentativeValue {
			get {
				return this.Values.First();
			}
		}

		/// <summary>
		/// 値と件数リスト
		/// </summary>
		public IEnumerable<ValueCountPair<string>> Values {
			get;
		}

		/// <summary>
		/// 複数の値が含まれているか
		/// </summary>
		public bool HasMultipleValues {
			get {
				return this.Values.Count() >= 2;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="values">値と件数リスト</param>
		public MediaFileProperty(string title, IEnumerable<ValueCountPair<string>> values) {
			this.Title = title;
			this.Values = values;
		}
	}

	/// <summary>
	/// 値と件数のペア
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal struct ValueCountPair<T> : IEquatable<ValueCountPair<T>> {
		public ValueCountPair(T value, int count) {
			this.Value = value;
			this.Count = count;
		}

		/// <summary>
		/// 値
		/// </summary>
		public T Value {
			get;
		}

		/// <summary>
		/// 件数
		/// </summary>
		public int Count {
			get;
		}

		public bool Equals(ValueCountPair<T> other) {
			if (other.Count == this.Count && other.Count == this.Count) {
				return true;
			}
			return false;
		}
	}
}
