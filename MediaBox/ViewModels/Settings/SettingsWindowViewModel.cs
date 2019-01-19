using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Settings.Pages;

namespace SandBeige.MediaBox.ViewModels.Settings {
	internal class SettingsWindowViewModel : ViewModelBase {

		public ISettingsViewModel[] SettingsPages {
			get;
		}

		public IReactiveProperty<ISettingsViewModel> CurrentSettingsPage {
			get;
		} = new ReactiveProperty<ISettingsViewModel>();

		public SettingsWindowViewModel() {
			this.SettingsPages = new ISettingsViewModel[] {
				Get.Instance<GeneralSettingsViewModel>().AddTo(this.CompositeDisposable),
				Get.Instance<PathSettingsViewModel>().AddTo(this.CompositeDisposable),
				Get.Instance<ExternalToolsSettingsViewModel>().AddTo(this.CompositeDisposable)
			};

			this.CurrentSettingsPage.Value = this.SettingsPages.FirstOrDefault();
		}
	}
}
