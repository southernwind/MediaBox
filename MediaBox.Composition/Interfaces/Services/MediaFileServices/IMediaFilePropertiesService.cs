using System;

using SandBeige.MediaBox.Composition.Interfaces.Services.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces.Services.MediaFileServices {
	public interface IMediaFilePropertiesService {
		IObservable<MediaFileUpdateNotificationArgs<AddTagNotificationDetail>> TagAdded {
			get;
		}
		IObservable<MediaFileUpdateNotificationArgs<RemoveTagNotificationDetail>> TagRemoved {
			get;
		}

		IObservable<MediaFileUpdateNotificationArgs<SetRateNotificationDetail>> RateSet {
			get;
		}

		void AddTag(long[] mediaFileIds, string tagName);
		void RemoveTag(long[] mediaFileIds, string tagName);

		/// <summary>
		/// 評価設定
		/// </summary>
		/// <param name="mediaFileIds">評価設定対象ID</param>
		/// <param name="rate">評価</param>
		void SetRate(long[] mediaFileIds, int rate);
	}
}