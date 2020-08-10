using System;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Notification {
	public interface INotificationManager {
		IObservable<INotification> OnNotify {
			get;
		}

		void Notify(INotification notification);
	}
}