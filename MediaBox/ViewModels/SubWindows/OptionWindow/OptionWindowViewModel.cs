using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow.Pages;

namespace SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow {
	internal class OptionWindowViewModel : ViewModelBase {

		public ISettingsViewModel[] SettingsPages {
			get;
		}

		public IReactiveProperty<ISettingsViewModel> CurrentSettingsPage {
			get;
		} = new ReactiveProperty<ISettingsViewModel>();

		public OptionWindowViewModel() {
			this.SettingsPages = new ISettingsViewModel[] {
				Get.Instance<GeneralSettingsViewModel>().AddTo(this.CompositeDisposable),
				Get.Instance<PathSettingsViewModel>().AddTo(this.CompositeDisposable)
			};

			this.CurrentSettingsPage.Value = this.SettingsPages.FirstOrDefault();
		}
	}
}
