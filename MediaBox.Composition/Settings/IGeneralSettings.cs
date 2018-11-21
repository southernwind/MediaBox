using Reactive.Bindings;
using System;
using System.ComponentModel;

namespace SandBeige.MediaBox.Composition.Settings {
	public interface IGeneralSettings :INotifyPropertyChanged,IDisposable{
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
		/// 管理対象拡張子
		/// </summary>
		IReactiveProperty<string[]> TargetExtensions {
			get;
			set;
		}


		IReactiveProperty<string> BingMapApiKey {
			get;
			set;
		}
	}
}