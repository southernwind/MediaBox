using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Livet;

namespace SandBeige.MediaBox.Composition.Settings.Objects {

	/// <summary>
	/// 設定基底クラス
	/// </summary>
	public class SettingsBase : NotificationObject, ISettingsBase {
		/// <summary>
		/// 設定差分出力
		/// </summary>
		/// <returns>
		/// プロパティ名をキーとする設定値Dictionary
		/// </returns>
		public Dictionary<string, dynamic> Export() {
			return this.GetType()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.PropertyType.GetInterfaces().Any(t => t == typeof(ISettingsItem)))
				.Select(x => (x.Name, SettingsItem: x.GetValue(this) as ISettingsItem))
				.Where(x => x.SettingsItem.HasDiff())
				.ToDictionary(x => x.Name, x => ((dynamic)x.SettingsItem).Value);
		}

		/// <summary>
		/// 設定差分読み込み
		/// </summary>
		/// <param name="settings">
		/// プロパティ名をキーとする設定値Dictionary
		/// </param>
		public void Import(Dictionary<string, dynamic> settings) {
			foreach (var si in this.GetType()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.PropertyType.GetInterfaces().Any(t => t == typeof(ISettingsItem)))
				.Select(x => (x.Name, SettingsItem: x.GetValue(this) as ISettingsItem))) {
				if (settings.TryGetValue(si.Name, out var value)) {
					si.SettingsItem.SetValue(value);
				}
			}
		}

		/// <summary>
		/// デフォルトロード
		/// </summary>
		public void LoadDefault() {
			foreach (var si in this.GetType()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.PropertyType.GetInterfaces().Any(t => t == typeof(ISettingsItem)))
				.Select(x => (x.Name, SettingsItem: x.GetValue(this) as ISettingsItem))) {
				si.SettingsItem.SetDefaultValue();
			}
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
			foreach (var si in this.GetType()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.PropertyType.GetInterfaces().Any(t => t == typeof(ISettingsItem)))
				.Select(x => (x.Name, SettingsItem: x.GetValue(this) as ISettingsItem))) {
				si.SettingsItem?.Dispose();
			}
		}
	}
}
