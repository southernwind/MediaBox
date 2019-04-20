using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイル情報
	/// </summary>
	/// <remarks>
	/// 複数のメディアファイルの情報をまとめて閲覧できるようにする
	/// </remarks>
	internal class MediaFileInformation : ModelBase {
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
		public IReactiveProperty<IEnumerable<GpsLocation>> Locations {
			get;
		} = new ReactivePropertySlim<IEnumerable<GpsLocation>>();

		/// <summary>
		/// 評価平均
		/// </summary>
		public IReactiveProperty<double> AverageRate {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileInformation() {
			this.FilesCount = this.Files.Select(x => x.Count()).ToReadOnlyReactivePropertySlim();
			this.RepresentativeMediaFile = this.Files.Select(Enumerable.FirstOrDefault).ToReadOnlyReactivePropertySlim();
			this.Files
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => {
					this.UpdateTags();
					this.UpdateProperties();
					this.UpdateMetadata();
					this.UpdateLocations();
					this.UpdateRate();
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// 対象ファイルすべての評価設定
		/// </summary>
		/// <param name="rate"></param>
		public void SetRate(int rate) {
			lock (this.DataBase) {
				using (var tran = this.DataBase.Database.BeginTransaction()) {
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
				using (var tran = this.DataBase.Database.BeginTransaction()) {
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
				using (var tran = this.DataBase.Database.BeginTransaction()) {
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
			}
			this.UpdateTags();
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

			lock (this.DataBase) {
				var jpegs = this.DataBase
					.MediaFiles
					.Where(x => x.Jpeg != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.Jpeg)
					.Select(x => x.Jpeg)
					.ToList();
				var pngs = this.DataBase
					.MediaFiles
					.Where(x => x.Png != null)
					.Where(x => ids.Contains(x.MediaFileId))
					.Include(x => x.Png)
					.Select(x => x.Png)
					.ToList();
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
						).Where(x => x.Values.Any(v => v.Value != null));
			}
		}

		/// <summary>
		/// 座標の更新
		/// </summary>
		private void UpdateLocations() {
			this.Locations.Value =
				this.Files
					.Value
					.Select(x => x.Location)
					.Where(x => x != null);
		}

		/// <summary>
		/// 評価の更新
		/// </summary>
		private void UpdateRate() {
			var list = this.Files
					.Value
					.Where(x => x.Rate != 0)
					.Select(x => x.Rate);
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
