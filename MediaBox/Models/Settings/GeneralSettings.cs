
using System;

using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.Models.Settings {
	public class GeneralSettings : NotificationObject, IGeneralSettings {
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

		/// <summary>
		/// マップピンサイズ
		/// </summary>
		public IReactiveProperty<int> MapPinSize {
			get;
			set;
		} = new ReactiveProperty<int>(150);

		public IReactiveProperty<DisplayMode> DisplayMode {
			get;
			set;
		} = new ReactiveProperty<DisplayMode>();

		/// <summary>
		/// ソート設定
		/// </summary>
		public IReactiveProperty<SortDescriptionParams[]> SortDescriptions {
			get;
			set;
		} = new ReactiveProperty<SortDescriptionParams[]>(Array.Empty<SortDescriptionParams>());

		public void Dispose() {
			this.TargetExtensions?.Dispose();
			this.BingMapApiKey?.Dispose();
			this.ThumbnailWidth?.Dispose();
			this.ThumbnailHeight?.Dispose();
			this.MapPinSize?.Dispose();
			this.DisplayMode?.Dispose();
		}
	}
}
