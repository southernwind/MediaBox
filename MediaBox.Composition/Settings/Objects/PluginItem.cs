using System;
using System.IO;

using SandBeige.MediaBox.Composition.Interfaces.Models.Plugin;

namespace SandBeige.MediaBox.Composition.Settings.Objects {
	public class PluginItem {
		/// <summary>
		/// プラグインDLLファイルパス
		/// </summary>
		public string PluginDllPath {
			get;
			set;
		}

		/// <summary>
		/// プラグイン型
		/// </summary>
		public string PluginClassName {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="plugin">プラグインインスタンス</param>
		public PluginItem(IPlugin plugin) {
			var type = plugin.GetType();
			this.PluginDllPath = Path.GetFileNameWithoutExtension(type.Assembly.Location);
			this.PluginClassName = type.FullName!;
		}

		[Obsolete("for serialize")]
		public PluginItem() {
		}

		public bool IsSamePlugin(IPlugin plugin) {
			var type = plugin.GetType();
			return this.PluginDllPath == Path.GetFileNameWithoutExtension(type.Assembly.Location) && this.PluginClassName == type.FullName;
		}
	}
}
