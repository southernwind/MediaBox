using System;
using System.ComponentModel;

namespace SandBeige.MediaBox.Composition.Settings {
	public interface IGeneralSettings :INotifyPropertyChanged,IDisposable{
		/// <summary>
		/// データベースファイルパス
		/// </summary>
		string DataBaseFilePath {
			get;
			set;
		}

		/// <summary>
		/// サムネイルディレクトリパス
		/// </summary>
		string ThumbnailDirectoryPath {
			get;
			set;
		}

		/// <summary>
		/// 管理対象拡張子
		/// </summary>
		string[] TargetExtensions {
			get;
			set;
		}

		
		string BingMapApiKey {
			get;
			set;
		}
	}
}