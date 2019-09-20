using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Plugins;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Plugin {
	/// <summary>
	/// プラグイン管理
	/// </summary>
	internal class PluginManager : ModelBase {
		/// <summary>
		/// 利用可能プラグインリスト
		/// </summary>
		private readonly ReactiveCollection<IPlugin> _availablePluginList = new ReactiveCollection<IPlugin>();

		/// <summary>
		/// プラグインリスト
		/// </summary>
		public ReadOnlyReactiveCollection<PluginModel> PluginList {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PluginManager() {
			this.LoadPluginList();

			this.PluginList =
				this._availablePluginList
					.ToReadOnlyReactiveCollection(x => new PluginModel(x));
		}

		/// <summary>
		/// プラグインリスト読み込み
		/// </summary>
		private void LoadPluginList() {
			var dir = this.Settings.PathSettings.PluginDirectoryPath.Value;
			var plugins = Directory
				.GetFiles(dir, "*.dll")
				.SelectMany(path => Assembly.LoadFrom(path).GetTypes().Select(x => (path, type: x)))
				.Where(x => x.type.GetInterfaces().Any(x => x == typeof(IPlugin)))
				.Select(x => (IPlugin)Activator.CreateInstance(x.type))
				.ToArray();

			this._availablePluginList.Clear();
			this._availablePluginList.AddRange(plugins);
		}
	}
}
