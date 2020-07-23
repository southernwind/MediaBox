using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Notification;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels {
	public class LogViewerViewModel : ViewModelBase {
		public ReadOnlyReactiveCollection<INotification> Log {
			get;
		}

		public LogViewerViewModel() {
			var notificationManager = Get.Instance<NotificationManager>();

			var log = new ReactiveCollection<INotification>();
			this.Log = log.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			notificationManager.OnNotify.Subscribe(x => {
				log.Add(x);
			}).AddTo(this.CompositeDisposable);
		}
	}
}
