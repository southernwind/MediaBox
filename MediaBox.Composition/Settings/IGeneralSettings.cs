using System;

namespace SandBeige.MediaBox.Composition.Settings {
	public interface IGeneralSettings :IDisposable{
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
	}
}