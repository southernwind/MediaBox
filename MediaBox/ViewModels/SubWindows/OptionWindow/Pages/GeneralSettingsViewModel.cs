using Reactive.Bindings;
using SandBeige.MediaBox.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings.Extensions;

namespace SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow.Pages
{
    class GeneralSettingsViewModel:ViewModelBase {
		public ReactiveProperty<string> BingMapApiKey {
			get;
			private set;
		}

		
		public GeneralSettingsViewModel Initialize() {
			this.BingMapApiKey = this.Settings.GeneralSettings.ToReactivePropertyAsSynchronized(x => x.BingMapApiKey);
			return this;
		}
	}
}
