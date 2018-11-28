using Reactive.Bindings;
using System;
using System.ComponentModel;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// 一般設定
	/// </summary>
	public interface IGeneralSettings :INotifyPropertyChanged,IDisposable{

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

		/// <summary>
		/// サムネイル幅
		/// </summary>
		IReactiveProperty<int> ThumbnailWidth {
			get;
			set;
		}

		/// <summary>
		/// サムネイル高さ
		/// </summary>
		IReactiveProperty<int> ThumbnailHeight {
			get;
			set;
		}

	}
}