using System;
using System.ComponentModel;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// 一般設定
	/// </summary>
	public interface IGeneralSettings : INotifyPropertyChanged, IDisposable {

		/// <summary>
		/// 管理対象拡張子
		/// </summary>
		IReactiveProperty<string[]> TargetExtensions {
			get;
		}


		IReactiveProperty<string> BingMapApiKey {
			get;
		}

		/// <summary>
		/// サムネイル幅
		/// </summary>
		IReactiveProperty<int> ThumbnailWidth {
			get;
		}

		/// <summary>
		/// サムネイル高さ
		/// </summary>
		IReactiveProperty<int> ThumbnailHeight {
			get;
		}

		/// <summary>
		/// マップピンサイズ
		/// </summary>
		IReactiveProperty<int> MapPinSize {
			get;
		}

		IReactiveProperty<DisplayMode> DisplayMode {
			get;
		}
	}
}