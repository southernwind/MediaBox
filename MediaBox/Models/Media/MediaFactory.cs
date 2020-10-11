using System;
using System.Linq;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.Notification;
using SandBeige.MediaBox.Composition.Interfaces.Services.MediaFileServices;
using SandBeige.MediaBox.Composition.Interfaces.Services.Objects;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファクトリー
	/// </summary>
	/// <remarks>
	/// キャッシュ機構を持ったMediaFileModelのファクトリー
	/// ファイルパスをキーとして、同一ファイルを生成したことがあればキャッシュしているものを返す。
	/// GCされていたり、Disposeされていたりすると新しく生成しなおして返す。
	/// </remarks>
	public class MediaFactory : FactoryBase<string, IMediaFileModel>, IMediaFactory {
		private readonly ISettings _settings;
		private readonly ILogging _logging;
		private readonly INotificationManager _notificationManager;
		private readonly IImageThumbnailService _imageThumbnailService;
		private readonly IVideoThumbnailService _videoThumbnailService;

		public MediaFactory(ISettings settings, ILogging logging, INotificationManager notificationManager, IImageThumbnailService imageThumbnailService, IVideoThumbnailService videoThumbnailService, IMediaFilePropertiesService mediaFilePropertiesService) {
			this._settings = settings;
			this._logging = logging;
			this._notificationManager = notificationManager;
			this._imageThumbnailService = imageThumbnailService;
			this._videoThumbnailService = videoThumbnailService;

			this.RegisterPropertyUpdateEvent(mediaFilePropertiesService.RateSet, (target, detail) => target.Rate = detail.Rate);
			this.RegisterPropertyUpdateEvent(mediaFilePropertiesService.TagAdded, (target, detail) => target.AddTag(detail.TagName));
			this.RegisterPropertyUpdateEvent(mediaFilePropertiesService.TagRemoved, (target, detail) => target.RemoveTag(detail.TagName));
		}

		/// <summary>
		/// <see cref="IMediaFileModel"/>の取得
		/// </summary>
		/// <param name="key">ファイルパス</param>
		/// <returns>生成された<see cref="IMediaFileModel"/></returns>
		public IMediaFileModel Create(string key) {
			var mf = this.Create<string, IMediaFileModel>(key);
			return mf;
		}

		/// <summary>
		/// 新規生成時の関数
		/// </summary>
		/// <typeparam name="TKey">ファイルパスの型(<see cref="string"/>)</typeparam>
		/// <typeparam name="TValue">生成されるインスタンスの型(<see cref="IMediaFileModel"/>)</typeparam>
		/// <param name="key">ファイルパス</param>
		/// <returns>生成された<see cref="IMediaFileModel"/></returns>
		protected override IMediaFileModel CreateInstance<TKey, TValue>(TKey key) {
			// 拡張子で動画か画像かの判定を行う。
			if (key.IsVideoExtension(this._settings)) {
				var instance = new VideoFileModel(key, this._settings, this._logging, this._notificationManager, this._videoThumbnailService);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			} else {
				var instance = new ImageFileModel(key, this._settings, this._logging, this._imageThumbnailService);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			}
		}

		/// <summary>
		/// プロパティ更新イベント登録
		/// </summary>
		/// <typeparam name="T">更新イベント引数の型</typeparam>
		/// <param name="updateTrigger">トリガー</param>
		/// <param name="updateFunc">更新処理</param>
		private void RegisterPropertyUpdateEvent<T>(IObservable<MediaFileUpdateNotificationArgs<T>> updateTrigger, Action<IMediaFileModel, T> updateFunc) {
			updateTrigger
				.Subscribe(x => {
					var updateTargets = this.Pool.Values.Select(v => {
						v.TryGetTarget(out var t);
						return t;
					}).Where(t => x.TargetMediaFileId.OfType<long?>().Contains(t?.MediaFileId));

					foreach (var target in updateTargets) {
						updateFunc(target!, x.Detail);
					}
				});

		}
	}
}