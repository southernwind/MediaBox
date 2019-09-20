using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Plugin;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Plugin;

namespace SandBeige.MediaBox.ViewModels.Settings.Pages {
	/// <summary>
	/// プラグイン設定ViewModel
	/// </summary>
	internal class PluginSettingsViewModel : ViewModelBase, ISettingsViewModel {
		/// <summary>
		/// 設定名
		/// </summary>
		public string Name {
			get;
		}


		public ReadOnlyReactiveCollection<PluginViewModel> PluginList {
			get;
		}

		public IReactiveProperty<PluginViewModel> CurrentPlugin {
			get;
		} = new ReactivePropertySlim<PluginViewModel>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PluginSettingsViewModel() {
			this.Name = "プラグイン設定";
			var pm = Get.Instance<PluginManager>();
			this.PluginList = pm.PluginList.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create).AddTo(this.CompositeDisposable);
			this.CurrentPlugin.Value = this.PluginList.FirstOrDefault();
		}
	}
}
