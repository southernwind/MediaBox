using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.About;
using SandBeige.MediaBox.ViewModels.Settings;
using SandBeige.MediaBox.Views;
using SandBeige.MediaBox.Views.Settings;

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// ナビゲーションウィンドウViewModel
	/// </summary>
	internal class NavigationMenuViewModel : ViewModelBase {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NavigationMenuViewModel() {
			this.SettingsWindowOpenCommand.Subscribe(() => {
				using (var vm = Get.Instance<SettingsWindowViewModel>()) {
					var message = new TransitionMessage(typeof(SettingsWindow), vm, TransitionMode.Modal);
					this.Settings.Save();
					this.Messenger.Raise(message);
				}
			}).AddTo(this.CompositeDisposable);

			this.AboutWindowOpenCommand.Subscribe(() => {
				using (var vm = Get.Instance<AboutWindowViewModel>()) {
					var message = new TransitionMessage(typeof(AboutWindow), vm, TransitionMode.Modal);
					this.Messenger.Raise(message);
				}
			}).AddTo(this.CompositeDisposable);
		}

		#region WindowOpenCommand


		#region SettingsWindowOpenCommand
		/// <summary>
		/// 設定ウィンドウオープンコマンド
		/// </summary>
		public ReactiveCommand SettingsWindowOpenCommand {
			get;
		} = new ReactiveCommand();

		#endregion

		/// <summary>
		/// 概要ウィンドウオープンコマンド
		/// </summary>
		public ReactiveCommand AboutWindowOpenCommand {
			get;
		} = new ReactiveCommand();

		#endregion
	}
}
