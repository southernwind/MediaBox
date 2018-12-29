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
		/// パス設定
		/// </summary>
		IPathSettings PathSettings {
			get;
		}

		/// <summary>
		/// テスト用設定
		/// </summary>
		IForTestSettings ForTestSettings {
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
