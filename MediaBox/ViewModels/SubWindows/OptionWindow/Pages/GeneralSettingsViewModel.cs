using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;

namespace SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow.Pages {
	internal class GeneralSettingsViewModel : ViewModelBase, ISettingsViewModel {
		public string Name {
			get;
		}

		public ReactiveProperty<string> BingMapApiKey {
			get;
			private set;
		}

		public GeneralSettingsViewModel() {
			this.Name ="一般設定";
		}

		public GeneralSettingsViewModel Initialize() {
			this.BingMapApiKey = this.Settings.GeneralSettings.BingMapApiKey.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			return this;
		}
	}
}
