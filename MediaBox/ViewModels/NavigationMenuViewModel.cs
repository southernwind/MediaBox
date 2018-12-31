using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow;

namespace SandBeige.MediaBox.ViewModels {
	internal class NavigationMenuViewModel : ViewModelBase {
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
			}).AddTo(this.CompositeDisposable);
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
