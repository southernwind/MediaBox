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
	}
}