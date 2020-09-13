using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Plugin;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Plugin {
	/// <summary>
	/// プラグイン管理
	/// </summary>
	public class PluginManager : ModelBase, IPluginManager {
		private readonly ISettings _settings;
		/// <summary>
		/// 利用可能プラグインリスト
		/// </summary>
		private readonly ReactiveCollection<IPlugin> _availablePluginList = new ReactiveCollection<IPlugin>();

		/// <summary>
		/// プラグインリスト
		/// </summary>
		public ReadOnlyReactiveCollection<IPluginModel> PluginList {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PluginManager(ISettings settings) {
			this._settings = settings;
			this.LoadPluginList();

			this.PluginList =
				this._availablePluginList
					.ToReadOnlyReactiveCollection<IPlugin, IPluginModel>(x => new PluginModel(x, settings));
		}

		/// <summary>
		/// プラグインリスト読み込み
		/// </summary>
		private void LoadPluginList() {
			var dir = this._settings.PathSettings.PluginDirectoryPath.Value;
			var plugins = Directory
				.GetFiles(dir, "*.dll")
				.SelectMany(path => Assembly.LoadFrom(path).GetTypes().Select(x => (path, type: x)))
				.Where(x => x.type.GetInterfaces().Any(i => i == typeof(IPlugin)))
				.Select(x => (IPlugin)Activator.CreateInstance(x.type)!)
				.ToArray();

			this._availablePluginList.Clear();
			this._availablePluginList.AddRange(plugins);
		}
	}
}
