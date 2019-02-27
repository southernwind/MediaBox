
using System;

using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Models.States {
	/// <summary>
	/// アルバムの状態
	/// </summary>
	public class AlbumStates : SettingsBase {
		/// <summary>
		/// カレント条件
		/// </summary>
		public SettingsItem<int?> CurrentFilteringCondition {
			get;
		} = new SettingsItem<int?>(null);

		/// <summary>
		/// フィルター条件リスト
		/// </summary>
		public SettingsCollection<int> FilteringConditions {
			get;
		} = new SettingsCollection<int>(Array.Empty<int>());

		/// <summary>
		/// デフォルトロード
		/// </summary>
		public void LoadDefault() {
			this.CurrentFilteringCondition.SetDefaultValue();
			this.FilteringConditions.SetDefaultValue();
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
		}
	}
}
