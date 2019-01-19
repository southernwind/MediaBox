using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Settings;
using SandBeige.MediaBox.Views.Settings;

namespace SandBeige.MediaBox.ViewModels {
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

		#endregion
	}
}
