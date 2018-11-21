using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SandBeige.MediaBox.Models;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow.Pages;
using SandBeige.MediaBox.Repository;
using Unity;

namespace SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow {
	internal class OptionWindowViewModel : ViewModelBase {

		/// <summary>
		/// 一般設定ViewModel
		/// </summary>
		public GeneralSettingsViewModel GeneralSettingsViewModel {
			get;
		}

		public OptionWindowViewModel() {
			this.GeneralSettingsViewModel = UnityConfig.UnityContainer.Resolve<GeneralSettingsViewModel>().Initialize();
		}

		public void Initialize() {
		}
	}
}
