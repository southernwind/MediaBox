using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Services.MediaFileServices;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables.Metadata;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Services;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイル情報
	/// </summary>
	/// <remarks>
	/// 複数のメディアファイルの情報をまとめて閲覧できるようにする
	/// </remarks>
	public class MediaFileInformation : ModelBase, IMediaFileInformation {
		private readonly IMediaBoxDbContext _rdb;
		private readonly IGeoCodingService _geoCodingService;
		private readonly IMediaFilePropertiesService _mediaFilePropertiesService;

		/// <summary>
		/// タグリスト
		/// </summary>
		public IReactiveProperty<IEnumerable<ValueCountPair<string>>> Tags {
			get;
		} = new ReactivePropertySlim<IEnumerable<ValueCountPair<string>>>();

		/// <summary>
		/// ファイルリスト
		/// </summary>
		public ReactiveCollection<IMediaFileModel> Files {
			get;
		} = new();

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
		public IReadOnlyReactiveProperty<IMediaFileModel?> RepresentativeMediaFile {
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
		public IReactiveProperty<IAddress?> Positions {
			get;
		} = new ReactivePropertySlim<IAddress?>();

		/// <summary>
		/// 評価平均
		/// </summary>
		public IReactiveProperty<double> AverageRate {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileInformation(IMediaBoxDbContext rdb, IGeoCodingService geoCodingService, VolatilityStateShareService volatilityStateShareService, IMediaFilePropertiesService mediaFilePropertiesService) {
			this._rdb = rdb;
			this._geoCodingService = geoCodingService;
			this._mediaFilePropertiesService = mediaFilePropertiesService;
			volatilityStateShareService.MediaFileModels.Subscribe(x => {
				this.Files.Clear();
				this.Files.AddRange(x);
			}).AddTo(this.CompositeDisposable);
			this.FilesCount = this.Files.ToCollectionChanged().Select(_ => this.Files.Count).ToReadOnlyReactivePropertySlim();
			this.RepresentativeMediaFile = this.Files.ToCollectionChanged().Select(_ => this.Files.FirstOrDefault()).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Files
				.ToCollectionChanged()
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

			// タグ更新時、表示更新
			var tagUpdateStream = this.Files
				.ObserveElementPropertyChanged()
				.Where(x => x.EventArgs.PropertyName == nameof(MediaFileModel.Tags));

			tagUpdateStream.Buffer(tagUpdateStream.Throttle(TimeSpan.FromMilliseconds(10)))
			.Where(x => this.Files.Select(m => m.MediaFileId).Intersect(x.Select(a => a.Sender.MediaFileId)).Any())
			.Subscribe(x => {
				this.UpdateTags();
			});
		}

		/// <summary>
		/// 対象ファイルすべてにタグ追加
		/// </summary>
		/// <param name="tagName">追加するタグ名</param>
		public void AddTag(string tagName) {
			var targetArray = this.Files.Select(x => x.MediaFileId).Where(x => x.HasValue).OfType<long>().ToArray();

			if (!targetArray.Any()) {
				return;
			}

			this._mediaFilePropertiesService.AddTag(targetArray, tagName);
		}

		/// <summary>
		/// 対象ファイルすべてからタグ削除
		/// </summary>
		/// <param name="tagName">削除するタグ名</param>
		public void RemoveTag(string tagName) {
			var targetArray = this.Files.Select(x => x.MediaFileId).Where(x => x.HasValue).OfType<long>().ToArray();

			if (!targetArray.Any()) {
				return;
			}
			this._mediaFilePropertiesService.RemoveTag(targetArray, tagName);
		}

		/// <summary>
		/// リバースジオコーディング
		/// </summary>
		/// <remarks>
		/// このクラスで扱っているファイルリストのうち、座標情報を持っているファイルに対してリバースジオコーディングを行う。
		/// キューに追加するだけなので、このメソッドを抜けた時点ではまだ実行されていない。そのうち完了する。
		/// </remarks>
		public void ReverseGeoCoding() {
			foreach (var m in this.Files.Where(x => x.Location != null)) {
				this._geoCodingService.Reverse(m.Location!);
			}
		}

		/// <summary>
		/// タグ更新
		/// </summary>
		private void UpdateTags() {
			this.Tags.Value =
				this.Files
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
					.SelectMany(x => x.Properties)
					.GroupBy(x => x.Title)
					.Select(x => new MediaFileProperty(
						x.Key,
						x.GroupBy(g => g.Value).Select(g => new ValueCountPair<string?>(g.Key, g.Count()))
					));

		}

		/// <summary>
		/// メタデータ更新
		/// </summary>
		private void UpdateMetadata() {
			var ids = this.Files.Select(x => x.MediaFileId).ToArray();

			List<Jpeg> jpegs;
			List<Png> pngs;
			List<Bmp> bmps;
			List<Gif> gifs;
			List<Heif> heifs;
			List<ICollection<VideoMetadataValue>> videoMetadata;
			lock (this._rdb) {
				jpegs = this._rdb
					.MediaFiles
					.Where(x => x.Jpeg != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.Jpeg)
					.Select(x => x.Jpeg!)
					.ToList();
				pngs = this._rdb
					.MediaFiles
					.Where(x => x.Png != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.Png)
					.Select(x => x.Png!)
					.ToList();
				bmps = this._rdb
					.MediaFiles
					.Where(x => x.Bmp != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.Bmp)
					.Select(x => x.Bmp!)
					.ToList();
				gifs = this._rdb
					.MediaFiles
					.Where(x => x.Gif != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.Gif)
					.Select(x => x.Gif!)
					.ToList();
				heifs = this._rdb
					.MediaFiles
					.Where(x => x.Heif != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.Heif)
					.Select(x => x.Heif!)
					.ToList();
				videoMetadata = this._rdb
					.MediaFiles
					.Where(x => x.VideoFile != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.VideoFile)
					.ThenInclude(x => x!.VideoMetadataValues)
					.Select(x => x.VideoFile!.VideoMetadataValues)
					.ToList();

				var positions = this._rdb
					.MediaFiles
					.Where(x => ids.Contains(x.MediaFileId))
					.Where(x => x.Position != null)
					.Include(x => x.Position)
					.ThenInclude(x => x!.NameDetails)
					.Include(x => x.Position)
					.ThenInclude(x => x!.Addresses)
					.Select(x => new { a = x.Position!.Addresses, b = x.Position })
					.AsEnumerable()
					.Select(x => x.b)
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
								.Select(x => new ValueCountPair<string?>(x.Key, x.Count()))
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
										.Select(x => new ValueCountPair<string?>(x.Key, x.Count()))
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
										.Select(x => new ValueCountPair<string?>(x.Key, x.Count()))
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
										.Select(x => new ValueCountPair<string?>(x.Key, x.Count()))
								)
							)
					).Union(
						typeof(Heif)
							.GetProperties()
							.Select(p =>
								new MediaFileProperty(
									p.Name,
									heifs
										.Select(x => p.GetValue(x)?.ToString())
										.GroupBy(x => x)
										.Select(x => new ValueCountPair<string?>(x.Key, x.Count()))
								)
							)
					).Union(
						videoMetadata
							.SelectMany(x => x)
							.GroupBy(x => x.Key)
							.Select(g => new MediaFileProperty(
								g.Key,
								g
									.GroupBy(x => x.Value)
									.Select(x => new ValueCountPair<string?>(x.Key, x.Count()))))
					).Where(x => x.Values.Any(v => v.Value != null));
		}

		/// <summary>
		/// 評価の更新
		/// </summary>
		private void UpdateRate() {
			var list = this.Files
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
			return $"<[{base.ToString()}] {this.RepresentativeMediaFile.Value?.FilePath} ({this.FilesCount.Value})>";
		}
	}
}
