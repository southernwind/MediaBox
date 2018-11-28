using Livet;
using Reactive.Bindings;
using SandBeige.MediaBox.Composition.Settings;
using System;
using System.IO;

namespace SandBeige.MediaBox.Models.Settings {
	public class GeneralSettings : NotificationObject,IGeneralSettings {
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

		/// <summary>
		/// サムネイル幅
		/// </summary>
		public IReactiveProperty<int> ThumbnailWidth {
			get;
			set;
		} = new ReactiveProperty<int>(200);
		
		/// <summary>
		/// サムネイル高さ
		/// </summary>
		public IReactiveProperty<int> ThumbnailHeight {
			get;
			set;
		} = new ReactiveProperty<int>(200);

		public void Dispose() {
		}
	}
}
