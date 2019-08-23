using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace SandBeige.MediaBox.Models.Notification {
	/// <summary>
	/// 通知管理
	/// </summary>
	internal class NotificationManager {
		private readonly Subject<INotification> _onNotifySubject = new Subject<INotification>();
		/// <summary>
		/// 通知
		/// </summary>
		public IObservable<INotification> OnNotify {
			get {
				return this._onNotifySubject.AsObservable();
			}
		}

		/// <summary>
		/// 通知
		/// </summary>
		/// <param name="notification">通知内容</param>
		public void Notify(INotification notification) {
			this._onNotifySubject.OnNext(notification);
		}
	}
}
