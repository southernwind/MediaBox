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
using SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow;
using Reactive.Bindings;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels {
	internal class NavigationMenuViewModel : ViewModelBase {
		public void Initialize() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NavigationMenuViewModel() {
			this.OptionWindowOpenCommand.Subscribe(() => {
				using (var vm = Get.Instance<OptionWindowViewModel>()) {
					var message = new TransitionMessage(typeof(Views.SubWindows.OptionWindow.OptionWindow), vm, TransitionMode.Modal);
					this.Settings.Save();
					this.Messenger.Raise(message);
				}
			});
		}

		#region WindowOpenCommand


		#region OptionWindowOpenCommand
		/// <summary>
		/// オプションオープンコマンド
		/// </summary>
		public ReactiveCommand OptionWindowOpenCommand {
			get;
		} = new ReactiveCommand();

		#endregion

		#endregion
	}
}
