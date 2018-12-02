using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet.Messaging.IO;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Utilities;


namespace SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow.Pages
{
	internal class PathSettingsViewModel : ViewModelBase, ISettingsViewModel {
		public string Name {
			get;
		}

		public PathSettingsViewModel() {
			this.Name = "パス設定";
		}
	}
}
