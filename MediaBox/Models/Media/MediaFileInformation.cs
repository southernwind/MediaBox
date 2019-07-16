using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

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
				using var tran = this.DataBase.Database.BeginTransaction();
				var targetArray = this.Files.Value;
				var mfs =
					this.DataBase
						.MediaFiles
						.Where(x => targetArray.Select(m => m.MediaFileId.Value).Contains(x.MediaFileId))
						.ToList();

				foreach (var mf in mfs) {
					mf.Rate = rate;
				}
				this.DataBase.SaveChanges();
				tran.Commit();

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
				using var tran = this.DataBase.Database.BeginTransaction();
				// すでに同名タグがあれば再利用、なければ作成
				var tagRecord = this.DataBase.Tags.SingleOrDefault(x => x.TagName == tagName) ?? new Tag { TagName = tagName };
				var mfs =
					this.DataBase
						.MediaFiles
						.Include(f => f.MediaFileTags)
						.Where(x =>
							targetArray.Select(m => m.MediaFileId.Value).Contains(x.MediaFileId) &&
							!x.MediaFileTags.Select(t => t.Tag.TagName).Contains(tagName))
						.ToList();

				foreach (var mf in mfs) {
					mf.MediaFileTags.Add(new MediaFileTag {
						Tag = tagRecord
					});
				}

				this.DataBase.SaveChanges();
				tran.Commit();

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
				using var tran = this.DataBase.Database.BeginTransaction();
				var mfts = this.DataBase
					.MediaFileTags
					.Where(x =>
						targetArray.Select(m => m.MediaFileId.Value).Contains(x.MediaFileId) &&
						x.Tag.TagName == tagName
					);

				// RemoveRangeを使うと、以下のような1件ずつのDELETE文が発行される。2,3千件程度では気にならない速度が出ている。
				// Executed DbCommand (0ms) [Parameters=[@p0='?', @p1='?'], CommandType='Text', CommandTimeout='30']
				// DELETE FROM "MediaFileTags"
				// WHERE "MediaFileId" = @p0 AND "TagId" = @p1;
				// 直接SQLを書けば1文で削除できるので早いはずだけど、保守性をとってとりあえずこれでいく。
				this.DataBase.MediaFileTags.RemoveRange(mfts);
				this.DataBase.SaveChanges();
				tran.Commit();

				foreach (var item in targetArray) {
					item.RemoveTag(tagName);
				}
			}
			this.UpdateTags();
		}

		public void OpenTagAlbum(string tag) {
			this._selector.SetDatabaseAlbumToCurrent($"タグ：{tag}", tag);
		}

		public void OpenPlaceAlbum(Address address) {
			this._selector.PositionSearchAlbumToCurrent($"場所：{address.Name}", address);
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
			foreach (var item in this.Files.Value) {
				item.CreateThumbnail();
			}
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
			lock (this.DataBase) {
				jpegs = this.DataBase
					.MediaFiles
					.Where(x => x.Jpeg != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.Jpeg)
					.Select(x => x.Jpeg)
					.ToList();
				pngs = this.DataBase
					.MediaFiles
					.Where(x => x.Png != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.Png)
					.Select(x => x.Png)
					.ToList();
				bmps = this.DataBase
					.MediaFiles
					.Where(x => x.Bmp != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.Bmp)
					.Select(x => x.Bmp)
					.ToList();
				gifs = this.DataBase
					.MediaFiles
					.Where(x => x.Gif != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.Gif)
					.Select(x => x.Gif)
					.ToList();
				videoMetadata = this.DataBase
					.MediaFiles
					.Where(x => x.VideoFile != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.VideoFile)
					.ThenInclude(x => x.VideoMetadataValues)
					.Select(x => x.VideoFile.VideoMetadataValues)
					.ToList();

				// 妙な書き方だけど、こうしないと勝手に気を利かせてAddressesのクエリを削りよる。
				var positions = this.DataBase
					.MediaFiles
					.Where(x => ids.Contains(x.MediaFileId))
					.Where(x => x.Position != null)
					.Include(x => x.Position)
					.ThenInclude(x => x.Addresses)
					.Select(x => new { a = x.Position.Addresses, b = x.Position })
					.AsEnumerable()
					.Select(x => x.b)
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
				} else {
					return this.Locations.FirstOrDefault().ToString();
				}
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
		/// <param name="displayName">場所名</param>
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
