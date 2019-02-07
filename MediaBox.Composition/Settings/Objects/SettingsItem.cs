using System;
using System.Collections.Generic;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Settings.Objects {
	/// <summary>
	/// 単一値の設定アイテム
	/// </summary>
	/// <typeparam name="T">型</typeparam>
	public class SettingsItem<T> : ReactiveProperty<T>, ISettingsItem<T> {
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

		/// <summary>
		/// デフォルト値との比較
		/// </summary>
		/// <returns>比較結果</returns>
		public bool HasDiff() {
			return !EqualityComparer<T>.Default.Equals(this.Value, this.DefaultValue);
		}

		/// <summary>
		/// 値の再設定
		/// </summary>
		/// <param name="value">設定する値</param>
		public void SetValue(dynamic value) {
			this.Value = value;
		}
	}
}
