using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Plugin;
using SandBeige.MediaBox.ViewModels.Plugin;

namespace SandBeige.MediaBox.ViewModels.Settings.Pages {
	/// <summary>
	/// プラグイン設定ViewModel
	/// </summary>
	public class PluginSettingsViewModel : ViewModelBase, ISettingsViewModel {
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
		public PluginSettingsViewModel(ViewModelFactory viewModelFactory, IPluginManager pluginManager) {
			this.Name = "プラグイン設定";
			this.PluginList = pluginManager.PluginList.ToReadOnlyReactiveCollection(viewModelFactory.Create).AddTo(this.CompositeDisposable);
			this.CurrentPlugin.Value = this.PluginList.FirstOrDefault();
		}
	}
}
