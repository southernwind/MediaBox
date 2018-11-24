using Reactive.Bindings;
using System;
using System.ComponentModel;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// パス設定
	/// </summary>
	public interface IPathSettings :INotifyPropertyChanged,IDisposable{
		/// <summary>
		/// データベースファイルパス
		/// </summary>
		IReactiveProperty<string> DataBaseFilePath {
			get;
			set;
		}

		/// <summary>
		/// サムネイルディレクトリパス
		/// </summary>
		IReactiveProperty<string> ThumbnailDirectoryPath {
			get;
			set;
		}

		/// <summary>
		/// 監視ディレクトリ
		/// </summary>
		ReactiveCollection<IMonitoringDirectory> MonitoringDirectories {
			get;
			set;
		}
	}
}