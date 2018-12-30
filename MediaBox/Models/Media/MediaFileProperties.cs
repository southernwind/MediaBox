using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルプロパティ一覧
	/// 複数のメディアファイルのプロパティをまとめて一つのプロパティとして閲覧できるようにする
	/// </summary>
	internal class MediaFileProperties : MediaFileCollection {
		/// <summary>
		/// タグリスト
		/// </summary>
		public ReactivePropertySlim<IEnumerable<ValueCountPair<string>>> Tags {
			get;
		} = new ReactivePropertySlim<IEnumerable<ValueCountPair<string>>>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileProperties() {
			this.Items.ToCollectionChanged().Subscribe(_ => this.UpdateTags());
			this.Items
				.ToReadOnlyReactiveCollection(x => {
					return x.Tags.ToCollectionChanged().Subscribe(_ => {
						this.UpdateTags();
					}).AddTo(this.CompositeDisposable);
				}, disposeElement: false).AddTo(this.CompositeDisposable)
				.DisposeWhenRemove()
				.AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// 対象ファイルすべてにタグ追加
		/// </summary>
		/// <param name="tagName">追加するタグ名</param>
		public void AddTag(string tagName) {
			using (var tran = this.DataBase.Database.BeginTransaction()) {
				// すでに同名タグがあれば再利用、なければ作成
				var tagRecord = this.DataBase.Tags.SingleOrDefault(x => x.TagName == tagName) ?? new Tag() { TagName = tagName };
				var targetArray = this.Items.Where(x => x.MediaFileId.HasValue && !x.Tags.Contains(tagName)).ToArray();
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

		/// <summary>
		/// 対象ファイルすべてからタグ削除
		/// </summary>
		/// <param name="tagName">削除するタグ名</param>
		public void RemoveTag(string tagName) {
			using (var tran = this.DataBase.Database.BeginTransaction()) {
				var targetArray = this.Items.Where(x => x.MediaFileId.HasValue && x.Tags.Contains(tagName)).ToArray();
				var mfs =
					this.DataBase
						.MediaFiles
						.Include(f => f.MediaFileTags)
						.Where(x =>
							targetArray.Select(m => m.MediaFileId.Value).Contains(x.MediaFileId) &&
							x.MediaFileTags.Select(t => t.Tag.TagName).Contains(tagName))
						.ToList();

				if (mfs.Count == 0) {
					return;
				}

				// DELETEはEntityFrameworkだと性能が出ないので直接SQLを書く
				var sql = $@"
DELETE
	FROM {nameof(this.DataBase.MediaFileTags)}
WHERE
	{nameof(MediaFileTag.MediaFileId)} in ({string.Join(",", mfs.Select(x => x.MediaFileId))}) AND
	{nameof(MediaFileTag.TagId)} in (
		SELECT DISTINCT {nameof(Tag.TagId)} 
		FROM {nameof(this.DataBase.Tags)}
		WHERE {nameof(MediaFileTag.Tag.TagName)}= {{0}}
	)
";
#pragma warning disable EF1000 // Possible SQL injection vulnerability.
				this.DataBase.Database.ExecuteSqlCommand(sql, tagName);
#pragma warning restore EF1000 // Possible SQL injection vulnerability.

				this.DataBase.SaveChanges();
				tran.Commit();

				foreach (var item in targetArray) {
					item.Tags.Remove(tagName);
				}
			}
		}

		/// <summary>
		/// タグ更新
		/// </summary>
		private void UpdateTags() {
			this.Tags.Value =
				this.Items
					.SelectMany(x => x.Tags)
					.GroupBy(x => x)
					.Select(x => new ValueCountPair<string>(x.Key, x.Count()));
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
