using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace SandBeige.MediaBox.ViewModels.Settings.Pages {
	/// <summary>
	/// 一般設定ViewModel
	/// </summary>
	internal class GeneralSettingsViewModel : ViewModelBase, ISettingsViewModel {
		/// <summary>
		/// 設定名
		/// </summary>
		public string Name {
			get;
		}

		/// <summary>
		/// Bing Map Api Key
		/// </summary>
		public IReactiveProperty<string> BingMapApiKey {
			get;
		}

		/// <summary>
		/// サムネイル幅
		/// </summary>
		public IReactiveProperty<int> ThumbnailWidth {
			get;
			set;
		}

		/// <summary>
		/// サムネイル高さ
		/// </summary>
		public IReactiveProperty<int> ThumbnailHeight {
			get;
			set;
		}

		/// <summary>
		/// マップピンサイズ
		/// </summary>
		public IReactiveProperty<int> MapPinSize {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GeneralSettingsViewModel() {
			this.Name = "一般設定";
			this.BingMapApiKey = this.Settings.GeneralSettings.BingMapApiKey.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.ThumbnailWidth = this.Settings.GeneralSettings.ThumbnailWidth.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.ThumbnailHeight = this.Settings.GeneralSettings.ThumbnailHeight.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.MapPinSize = this.Settings.GeneralSettings.MapPinSize.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
		}
	}
}
