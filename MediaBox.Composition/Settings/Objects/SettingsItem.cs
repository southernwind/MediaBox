using System;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Settings.Objects {
	/// <summary>
	/// 単一値の設定アイテム
	/// </summary>
	/// <typeparam name="T">型</typeparam>
	public class SettingsItem<T> : ReactiveProperty<T> {
		/// <summary>
		/// デフォルト値
		/// </summary>
		public T DefaultValue {
			get;
		}

		[Obsolete("for serialize")]
		public SettingsItem() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="defaultValue">デフォルト値</param>
		public SettingsItem(T defaultValue) {
			this.DefaultValue = defaultValue;
		}

		/// <summary>
		/// デフォルト値に戻す
		/// </summary>
		public void SetDefaultValue() {
			this.Value = this.DefaultValue;
		}
	}
}
