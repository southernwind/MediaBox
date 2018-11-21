using Livet;
using Reactive.Bindings;
using SandBeige.MediaBox.Composition.Settings;
using System;
using System.IO;

namespace SandBeige.MediaBox.Models.Settings {
	public class GeneralSettings : NotificationObject,IGeneralSettings {
		/// <summary>
		/// データベースファイルパス
		/// </summary>
		public IReactiveProperty<string> DataBaseFilePath {
			get;
			set;
		} = new ReactiveProperty<string>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.db"));

		/// <summary>
		/// サムネイルディレクトリパス
		/// </summary>
		public IReactiveProperty<string> ThumbnailDirectoryPath {
			get;
			set;
		} = new ReactiveProperty<string>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thumbs"));

		/// <summary>
		/// 管理対象拡張子
		/// </summary>
		public IReactiveProperty<string[]> TargetExtensions {
			get;
			set;
		} = new ReactiveProperty<string[]>(new[] { ".jpg", ".jpeg", ".png" });

		/// <summary>
		/// Bing Map Api Key
		/// </summary>
		public IReactiveProperty<string> BingMapApiKey {
			get;
			set;
		} = new ReactiveProperty<string>();

		public void Dispose() {
		}
	}
}
