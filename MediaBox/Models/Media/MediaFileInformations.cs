﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルプロパティ一覧
	/// 複数のメディアファイルのプロパティをまとめて一つのプロパティとして閲覧できるようにする
	/// </summary>
	internal class MediaFileInformations : ModelBase {
		/// <summary>
		/// タグリスト
		/// </summary>
		public IReactiveProperty<IEnumerable<ValueCountPair<string>>> Tags {
			get;
		} = new ReactivePropertySlim<IEnumerable<ValueCountPair<string>>>();

		public IReactiveProperty<IEnumerable<MediaFileModel>> Files {
			get;
		} = new ReactivePropertySlim<IEnumerable<MediaFileModel>>(Array.Empty<MediaFileModel>());

		public IReadOnlyReactiveProperty<int> FilesCount {
			get;
		}
		public IReadOnlyReactiveProperty<MediaFileModel> RepresentativeMediaFile {
			get;
		}

		/// <summary>
		/// プロパティ
		/// </summary>
		public IReactiveProperty<IEnumerable<MediaFileProperty>> Properties {
			get;
		} = new ReactivePropertySlim<IEnumerable<MediaFileProperty>>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileInformations() {
			this.FilesCount = this.Files.Select(x => x.Count()).ToReadOnlyReactivePropertySlim();
			this.RepresentativeMediaFile = this.Files.Select(Enumerable.FirstOrDefault).ToReadOnlyReactivePropertySlim();
			this.Files.Subscribe(x => {
				// TODO : Files変更時にDispose
				this.UpdateTags();
				this.UpdateProperties();

				foreach (var m in x) {
					m.Tags.ToCollectionChanged().Subscribe(_ => {
						this.UpdateTags();
					}).AddTo(this.CompositeDisposable);
					m.ObserveProperty(p => p.Properties).Subscribe(_ => this.UpdateProperties()).AddTo(this.CompositeDisposable);
				};
			}).AddTo(this.CompositeDisposable);
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
						item.Tags.Add(tagName);
					}
				}
			}
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
						item.Tags.Remove(tagName);
					}
				}
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
	internal class ValueCountPair<T> {
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
	}
}