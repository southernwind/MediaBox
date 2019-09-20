using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces.Plugins;
using SandBeige.MediaBox.Composition.Settings.Objects;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Plugin {

	/// <summary>
	/// プラグインモデル
	/// </summary>
	internal class PluginModel : ModelBase {
		/// <summary>
		/// プラグインインスタンス
		/// </summary>
		public IPlugin PluginInstance {
			get;
		}

		/// <summary>
		/// プラグインが有効か否か
		/// </summary>
		public IReactiveProperty<bool> IsEnabled {
			get;
		} = new ReactivePropertySlim<bool>();

		/// <summary>
		/// プラグイン
		/// </summary>
		/// <param name="plugin"></param>
		public PluginModel(IPlugin plugin) {
			this.PluginInstance = plugin;
			this.Settings.PluginSettings.PluginList.ToCollectionChanged().ToUnit()
				.Merge(Observable.Return(Unit.Default)).Subscribe(_ => {
					this.IsEnabled.Value = this.Settings.PluginSettings.PluginList.Any(x => x.IsSamePlugin(this.PluginInstance));
				});
		}

		/// <summary>
		/// 有効化
		/// </summary>
		public void ToEnable() {
			this.Settings.PluginSettings.PluginList.Add(new PluginItem(this.PluginInstance));
		}

		/// <summary>
		/// 無効化
		/// </summary>
		public void ToDisable() {
			var targets = this.Settings.PluginSettings.PluginList.Where(x => x.IsSamePlugin(this.PluginInstance));
			this.Settings.PluginSettings.PluginList.RemoveRange(targets);
		}
	}
}
