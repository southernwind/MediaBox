using System;
using System.Collections.Generic;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Settings.Objects {
	/// <summary>
	/// コレクションの設定値アイテム
	/// </summary>
	/// <typeparam name="T">型</typeparam>
	public class SettingsCollection<T> : ReactiveCollection<T> {
		/// <summary>
		/// デフォルト値
		/// </summary>
		public IEnumerable<T> DefaultValue {
			get;
		}

		[Obsolete("for serialize")]
		public SettingsCollection() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="defaultValue">デフォルト値</param>
		public SettingsCollection(params T[] defaultValue) {
			this.DefaultValue = defaultValue;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="defaultValue">デフォルト値</param>
		public SettingsCollection(IEnumerable<T> defaultValue) {
			this.DefaultValue = defaultValue;
		}

		/// <summary>
		/// デフォルト値に戻す
		/// </summary>
		public void SetDefaultValue() {
			this.Clear();
			foreach (var item in this.DefaultValue) {
				this.Add(item);
			}
		}
	}
}
