using System.Collections.Generic;
using System.ComponentModel;

namespace SandBeige.MediaBox.Composition.Settings.Objects {
	/// <summary>
	/// 設定クラスベースインターフェイス
	/// </summary>
	public interface ISettingsBase : INotifyPropertyChanged {
		/// <summary>
		/// 設定差分出力
		/// </summary>
		/// <returns>
		/// プロパティ名をキーとする設定値Dictionary
		/// </returns>
		Dictionary<string, dynamic> Export();

		/// <summary>
		/// 設定差分読み込み
		/// </summary>
		/// <param name="settings">
		/// プロパティ名をキーとする設定値Dictionary
		/// </param>
		void Import(Dictionary<string, dynamic> settings);
	}
}
