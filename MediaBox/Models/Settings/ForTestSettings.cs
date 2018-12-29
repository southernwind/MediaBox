﻿using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.Models.Settings {
	public class ForTestSettings : NotificationObject, IForTestSettings {
		public IReactiveProperty<bool> RunOnBackground {
			get;
			set;
		} = new ReactiveProperty<bool>(true);

		public void Dispose() {
			this.RunOnBackground?.Dispose();
		}
	}
}
