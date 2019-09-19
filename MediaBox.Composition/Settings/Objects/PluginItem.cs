using System;

namespace SandBeige.MediaBox.Composition.Settings.Objects {
	public class PluginItem {
		/// <summary>
		/// プラグイン名
		/// </summary>
		public string PluginName {
			get;
		}

		/// <summary>
		/// プラグインDLLファイルパス
		/// </summary>
		public string PluginDllPath {
			get;
		}

		/// <summary>
		/// プラグイン型
		/// </summary>
		public Type PluginType {
			get;
		}

		/// <summary>
		/// 有効か否か
		/// </summary>
		public bool IsEnabled {
			get;
		}
	}
}
