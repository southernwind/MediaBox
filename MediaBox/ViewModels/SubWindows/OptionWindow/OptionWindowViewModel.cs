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
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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
				UnityConfig.UnityContainer.Resolve<GeneralSettingsViewModel>().Initialize().AddTo(this.CompositeDisposable),
				UnityConfig.UnityContainer.Resolve<PathSettingsViewModel>().Initialize().AddTo(this.CompositeDisposable)
			};

			this.CurrentSettingsPage.Value = this.SettingsPages.FirstOrDefault();
		}

		/// <summary>
		/// ページ切り替え
		/// </summary>
		/// <typeparam name="T">変更ページ</typeparam>
		/// <returns>変更後ページViewModel</returns>
		public T ChangeCurrentPage<T>() {
			return (T)(this.CurrentSettingsPage.Value = this.SettingsPages.FirstOrDefault(x => x is T));
		}

		public void Initialize() {
		}
	}
}
