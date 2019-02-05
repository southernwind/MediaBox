using System;
using System.ComponentModel;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// パス設定
	/// </summary>
	public interface IPathSettings : INotifyPropertyChanged, IDisposable {
		/// <summary>
		/// データベースファイルパス
		/// </summary>
		IReactiveProperty<string> DataBaseFilePath {
			get;
		}

		/// <summary>
		/// サムネイルディレクトリパス
		/// </summary>
		IReactiveProperty<string> ThumbnailDirectoryPath {
			get;
		}

		/// <summary>
		/// 設定ロード
		/// </summary>
		/// <param name="pathSettings">読み込み元設定</param>
		void Load(IPathSettings pathSettings);

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		void LoadDefault();
	}
}