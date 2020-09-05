using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SandBeige.MediaBox.Composition.Settings.Objects {
	/// <summary>
	/// 単一値の設定アイテム
	/// </summary>
	/// <typeparam name="T">型</typeparam>
	/// <typeparam name="TKey">キーの型</typeparam>
	public class SettingsItemWithKey<TKey, T> : ISettingsItem<ConcurrentDictionary<TKey, T>>
		where TKey : notnull {
		/// <summary>
		/// デフォルト値生成関数
		/// </summary>
		private readonly Func<IEnumerable<KeyValuePair<TKey, T>>> _defaultValueCreator;

		/// <summary>
		/// 要素デフォルト値生成関数
		/// </summary>
		private readonly Func<T> _elementDefaultValueCreator;

		public ConcurrentDictionary<TKey, T> Value {
			get;
		} = new();

		[Obsolete("for serialize")]
		public SettingsItemWithKey() {
			this._elementDefaultValueCreator = null!;
			this._defaultValueCreator = null!;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="defaultValue">デフォルト値</param>
		/// <param name="elementDefaultValue">各要素のデフォルト値</param>
		public SettingsItemWithKey(IEnumerable<KeyValuePair<TKey, T>> defaultValue, T elementDefaultValue) {
			this._defaultValueCreator = () => defaultValue;
			this._elementDefaultValueCreator = () => elementDefaultValue;
		}

		/// <summary>
		/// インデクサ
		/// </summary>
		/// <param name="key">キー</param>
		/// <returns>値</returns>
		public T this[TKey key] {
			get {
				return this.Value.GetOrAdd(key, x => this._elementDefaultValueCreator());
			}
			set {
				if (!this.Value.TryAdd(key, value)) {
					this.Value[key] = value;
				}
			}
		}

		/// <summary>
		/// デフォルト値に戻す
		/// </summary>
		public void SetDefaultValue() {
			this.Value.Clear();
			foreach (var item in this._defaultValueCreator()) {
				this.Value.TryAdd(item.Key, item.Value);
			}
		}

		/// <summary>
		/// デフォルト値との比較
		/// </summary>
		/// <returns>比較結果</returns>
		public bool HasDiff() {
			return !this.Value.Select(x => new KeyValuePair<TKey, T>(x.Key, x.Value)).SequenceEqual(this._defaultValueCreator());
		}

		/// <summary>
		/// 値の再設定
		/// </summary>
		/// <param name="value">設定する値</param>
		public void SetValue(dynamic value) {
			this.Value.Clear();
			foreach (var item in (IEnumerable<KeyValuePair<TKey, T>>)value) {
				this.Value.TryAdd(item.Key, item.Value);
			}
		}

		public void Dispose() {
		}
	}
}
