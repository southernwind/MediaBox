using System;
using System.IO;

using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.Models.Settings {
	/// <summary>
	/// パス設定
	/// </summary>
	public class PathSettings : NotificationObject, IPathSettings {
		/// <summary>
		/// データベースファイルパス
		/// </summary>
		public IReactiveProperty<string> DataBaseFilePath {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// サムネイルディレクトリパス
		/// </summary>
		public IReactiveProperty<string> ThumbnailDirectoryPath {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// 設定ロード
		/// </summary>
		/// <param name="pathSettings">読み込み元設定</param>
		public void Load(IPathSettings pathSettings) {
			this.DataBaseFilePath.Value = pathSettings.DataBaseFilePath.Value;
			this.ThumbnailDirectoryPath.Value = pathSettings.ThumbnailDirectoryPath.Value;
		}

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		public void LoadDefault() {
			this.DataBaseFilePath.Value = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.db");
			this.ThumbnailDirectoryPath.Value = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thumbs");
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
			this.DataBaseFilePath?.Dispose();
			this.ThumbnailDirectoryPath?.Dispose();
		}

	}
}
