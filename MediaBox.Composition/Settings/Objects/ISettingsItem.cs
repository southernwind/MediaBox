using System;

namespace SandBeige.MediaBox.Composition.Settings.Objects {
	/// <summary>
	/// 設定アイテムインターフェイス
	/// </summary>
	/// <typeparam name="T">型</typeparam>
	public interface ISettingsItem<T> : ISettingsItem {
		/// <summary>
		/// 実際の値
		/// </summary>
		T Value {
			get;
		}
	}

	/// <summary>
	/// 設定アイテムインターフェイス
	/// </summary>
	public interface ISettingsItem : IDisposable {
		/// <summary>
		/// デフォルト値との比較
		/// </summary>
		/// <returns>比較結果</returns>
		bool HasDiff();

		/// <summary>
		/// 値の再設定
		/// </summary>
		/// <param name="value">設定する値</param>
		void SetValue(dynamic value);

		/// <summary>
		/// デフォルト値設定
		/// </summary>
		void SetDefaultValue();
	}
}
