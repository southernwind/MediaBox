using System;

namespace SandBeige.MediaBox.Composition.Settings {
	public interface ISettings : IDisposable {
		/// <summary>
		/// 一般設定
		/// </summary>
		IGeneralSettings GeneralSettings {
			get;
		}

		/// <summary>
		/// 保存
		/// </summary>
		void Save();

		/// <summary>
		/// 読み込み
		/// </summary>
		void Load();
	}
}
