using System;
using System.ComponentModel;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// 一般設定
	/// </summary>
	public interface IGeneralSettings : INotifyPropertyChanged, IDisposable {

		/// <summary>
		/// 管理対象拡張子
		/// </summary>
		ReactiveCollection<string> TargetExtensions {
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

		/// <summary>
		/// ソート設定
		/// </summary>
		IReactiveProperty<SortDescriptionParams[]> SortDescriptions {
			get;
		}

		/// <summary>
		/// 外部ツール
		/// </summary>
		ReactiveCollection<ExternalToolParams> ExternalTools {
			get;
		}

		/// <summary>
		/// 設定ロード
		/// </summary>
		/// <param name="generalSettings">読み込み元設定</param>
		void Load(IGeneralSettings generalSettings);

		/// <summary>
		/// 設定ロード
		/// </summary>
		void Load();
	}
}