
using System;

using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Settings {
	public class GeneralSettings : NotificationObject, IGeneralSettings {
		/// <summary>
		/// 管理対象拡張子
		/// </summary>
		public ReactiveCollection<string> TargetExtensions {
			get;
			set;
		} = new ReactiveCollection<string>();

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
		} = new ReactiveProperty<int>();

		/// <summary>
		/// サムネイル高さ
		/// </summary>
		public IReactiveProperty<int> ThumbnailHeight {
			get;
			set;
		} = new ReactiveProperty<int>();

		/// <summary>
		/// マップピンサイズ
		/// </summary>
		public IReactiveProperty<int> MapPinSize {
			get;
			set;
		} = new ReactiveProperty<int>();

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
		} = new ReactiveProperty<SortDescriptionParams[]>();

		/// <summary>
		/// 外部ツール
		/// </summary>
		public ReactiveCollection<ExternalToolParams> ExternalTools {
			get;
		} = new ReactiveCollection<ExternalToolParams>();

		/// <summary>
		/// 設定ロード
		/// </summary>
		/// <param name="generalSettings">読み込み元設定</param>
		public void Load(IGeneralSettings generalSettings) {
			this.TargetExtensions.Clear();
			this.TargetExtensions.AddRange(generalSettings.TargetExtensions);
			this.BingMapApiKey.Value = generalSettings.BingMapApiKey.Value;
			this.ThumbnailWidth.Value = generalSettings.ThumbnailWidth.Value;
			this.ThumbnailHeight.Value = generalSettings.ThumbnailHeight.Value;
			this.MapPinSize.Value = generalSettings.MapPinSize.Value;
			this.DisplayMode.Value = generalSettings.DisplayMode.Value;
			this.SortDescriptions.Value = generalSettings.SortDescriptions.Value;
		}

		/// <summary>
		/// 設定ロード
		/// </summary>
		public void LoadDefault() {
			this.TargetExtensions.Clear();
			this.TargetExtensions.AddRange(new[] { ".jpg", ".jpeg", ".png" });
			this.BingMapApiKey.Value = null;
			this.ThumbnailWidth.Value = 200;
			this.ThumbnailHeight.Value = 200;
			this.MapPinSize.Value = 15;
			this.DisplayMode.Value = Composition.Enum.DisplayMode.Library;
			this.SortDescriptions.Value = Array.Empty<SortDescriptionParams>();
		}

		public void Dispose() {
			this.TargetExtensions?.Dispose();
			this.BingMapApiKey?.Dispose();
			this.ThumbnailWidth?.Dispose();
			this.ThumbnailHeight?.Dispose();
			this.MapPinSize?.Dispose();
			this.DisplayMode?.Dispose();
			this.SortDescriptions?.Dispose();
			this.ExternalTools?.Dispose();
		}
	}
}
