namespace SandBeige.MediaBox.Controls.Objects {
	/// <summary>
	/// バインド表示用の汎用クラス
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BindingItem<T> {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get;
		}

		/// <summary>
		/// 値
		/// </summary>
		public T Value {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="displayName">表示名</param>
		/// <param name="value">値</param>
		public BindingItem(string displayName, T value) {
			this.DisplayName = displayName;
			this.Value = value;
		}
	}
}
