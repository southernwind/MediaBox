using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Microsoft.EntityFrameworkCore;

using SandBeige.MediaBox.Composition.Interfaces.Services.MediaFileServices;
using SandBeige.MediaBox.Composition.Interfaces.Services.Objects;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Services.MediaFileServices {
	public class MediaFilePropertiesService : IMediaFilePropertiesService {
		private readonly IMediaBoxDbContext _rdb;
		private readonly Subject<MediaFileUpdateNotificationArgs<AddTagNotificationDetail>> _tagAddedSubject;
		private readonly Subject<MediaFileUpdateNotificationArgs<RemoveTagNotificationDetail>> _tagRemovedSubject;
		private readonly Subject<MediaFileUpdateNotificationArgs<SetRateNotificationDetail>> _rateSetSubject;

		public IObservable<MediaFileUpdateNotificationArgs<AddTagNotificationDetail>> TagAdded {
			get {
				return this._tagAddedSubject.AsObservable();
			}
		}

		public IObservable<MediaFileUpdateNotificationArgs<RemoveTagNotificationDetail>> TagRemoved {
			get {
				return this._tagRemovedSubject.AsObservable();
			}
		}

		public IObservable<MediaFileUpdateNotificationArgs<SetRateNotificationDetail>> RateSet {
			get {
				return this._rateSetSubject.AsObservable();
			}
		}

		public MediaFilePropertiesService(IMediaBoxDbContext rdb) {
			this._rdb = rdb;
			this._tagAddedSubject = new();
			this._tagRemovedSubject = new();
			this._rateSetSubject = new();
		}

		/// <summary>
		/// タグ追加
		/// </summary>
		/// <param name="mediaFileIds">追加対象ID</param>
		/// <param name="tagName">タグ</param>
		public void AddTag(long[] mediaFileIds, string tagName) {
			lock (this._rdb) {
				using var tran = this._rdb.Database.BeginTransaction();
				// すでに同名タグがあれば再利用、なければ作成
				var tagRecord = this._rdb.Tags.SingleOrDefault(x => x.TagName == tagName) ?? new Tag { TagName = tagName };
				var mfs =
					this._rdb
						.MediaFiles
						.Include(f => f.MediaFileTags)
						.Where(x =>
							mediaFileIds.Contains(x.MediaFileId) &&
							!x.MediaFileTags.Select(t => t.Tag.TagName).Contains(tagName))
						.ToList();

				foreach (var mf in mfs) {
					mf.MediaFileTags.Add(new MediaFileTag {
						Tag = tagRecord
					});
				}

				this._rdb.SaveChanges();
				tran.Commit();
			}
			this._tagAddedSubject.OnNext(new MediaFileUpdateNotificationArgs<AddTagNotificationDetail>(mediaFileIds, new AddTagNotificationDetail(tagName)));
		}

		/// <summary>
		/// タグ削除
		/// </summary>
		/// <param name="mediaFileIds">削除対象ID</param>
		/// <param name="tagName">タグ</param>
		public void RemoveTag(long[] mediaFileIds, string tagName) {
			lock (this._rdb) {
				using var tran = this._rdb.Database.BeginTransaction();
				var mfts = this._rdb
					.MediaFileTags
					.Where(x =>
						mediaFileIds.Contains(x.MediaFileId) &&
						x.Tag.TagName == tagName
					);

				// RemoveRangeを使うと、以下のような1件ずつのDELETE文が発行される。2,3千件程度では気にならない速度が出ている。
				// Executed DbCommand (0ms) [Parameters=[@p0='?', @p1='?'], CommandType='Text', CommandTimeout='30']
				// DELETE FROM "MediaFileTags"
				// WHERE "MediaFileId" = @p0 AND "TagId" = @p1;
				// 直接SQLを書けば1文で削除できるので早いはずだけど、保守性をとってとりあえずこれでいく。
				this._rdb.MediaFileTags.RemoveRange(mfts);
				this._rdb.SaveChanges();
				tran.Commit();
			}
			this._tagRemovedSubject.OnNext(new MediaFileUpdateNotificationArgs<RemoveTagNotificationDetail>(mediaFileIds, new RemoveTagNotificationDetail(tagName)));
		}

		/// <summary>
		/// 評価設定
		/// </summary>
		/// <param name="mediaFileIds">評価設定対象ID</param>
		/// <param name="rate">評価</param>
		public void SetRate(long[] mediaFileIds, int rate) {
			lock (this._rdb) {
				using var tran = this._rdb.Database.BeginTransaction();
				var mfs =
					this._rdb
						.MediaFiles
						.Where(x => mediaFileIds.Contains(x.MediaFileId))
						.ToList();

				foreach (var mf in mfs) {
					mf.Rate = rate;
				}
				this._rdb.SaveChanges();
				tran.Commit();
			}
			this._rateSetSubject.OnNext(new MediaFileUpdateNotificationArgs<SetRateNotificationDetail>(mediaFileIds, new SetRateNotificationDetail(rate)));
		}
	}
}
