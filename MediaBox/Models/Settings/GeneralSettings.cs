using Livet;
using Reactive.Bindings;
using SandBeige.MediaBox.Composition.Settings;
using System;
using System.IO;

namespace SandBeige.MediaBox.Models.Settings {
	public class GeneralSettings : NotificationObject,IGeneralSettings {
		private string _bingMapApiKey;

		/// <summary>
		/// データベースファイルパス
		/// </summary>
		public string DataBaseFilePath {
			get;
			set;
		} = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.db");

		/// <summary>
		/// サムネイルディレクトリパス
		/// </summary>
		public string ThumbnailDirectoryPath {
			get;
			set;
		} = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thumbs");

		/// <summary>
		/// 管理対象拡張子
		/// </summary>
		public string[] TargetExtensions {
			get;
			set;
		} = new[] { ".jpg", ".jpeg", ".png" };

		/// <summary>
		/// Bing Map Api Key
		/// </summary>
		public string BingMapApiKey {
			get {
				return this._bingMapApiKey;
			}
			set {
				if(this._bingMapApiKey == value) {
					return;
				}
				this._bingMapApiKey = value;
				this.RaisePropertyChanged();
			}
		}

		public void Dispose() {
		}
	}
}
