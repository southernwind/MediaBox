using System;
using System.Collections.Generic;
using System.Linq;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Settings.Objects {
	/// <summary>
	/// コレクションの設定値アイテム
	/// </summary>
	/// <typeparam name="T">型</typeparam>
	public class SettingsCollection<T> : ReactiveCollection<T>, ISettingsItem<IEnumerable<T>> {
		/// <summary>
		/// デフォルト値
		/// </summary>
		public IEnumerable<T> DefaultValue {
			get;
		}

		/// <summary>
		/// 実際の値
		/// </summary>
		public IEnumerable<T> Value {
			get {
				return this;
			}
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

		/// <summary>
		/// デフォルト値との比較
		/// </summary>
		/// <returns>比較結果</returns>
		public bool HasDiff() {
			return !this.Value.SequenceEqual(this.DefaultValue);
		}

		/// <summary>
		/// 値の再設定
		/// </summary>
		/// <param name="value">設定する値</param>
		public void SetValue(dynamic value) {
			this.Clear();
			foreach (var item in value) {
				this.Add(item);
			}
		}
	}
}
