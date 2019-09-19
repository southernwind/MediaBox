
using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Plugins;
using SandBeige.MediaBox.Library.Extensions;

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

		/// <summary>
		/// プラグインリスト
		/// </summary>
		public ReactiveCollection<IPlugin> PluginList {
			get;
		} = new ReactiveCollection<IPlugin>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PluginSettingsViewModel() {
			this.Name = "プラグイン設定";
			this.LoadPluginList();
		}

		private void LoadPluginList() {
			var dir = this.Settings.PathSettings.PluginDirectoryPath.Value;
			var plugins =
				Directory
					.GetFiles(dir, "*.dll")
					.SelectMany(dll => Assembly.LoadFrom(dll).GetTypes())
					.Where(t => t.GetInterfaces().Any(x => x == typeof(IPlugin)))
					.Select(x => (IPlugin)Activator.CreateInstance(x))
					.ToArray();
			this.PluginList.Clear();
			this.PluginList.AddRange(plugins);
		}
	}
}
