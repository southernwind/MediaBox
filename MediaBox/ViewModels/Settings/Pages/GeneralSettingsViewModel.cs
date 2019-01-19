using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace SandBeige.MediaBox.ViewModels.Settings.Pages {
	internal class GeneralSettingsViewModel : ViewModelBase, ISettingsViewModel {
		public string Name {
			get;
		}

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

		public IReactiveProperty<int> MapPinSize {
			get;
			set;
		}


		public GeneralSettingsViewModel() {
			this.Name = "一般設定";
			this.BingMapApiKey = this.Settings.GeneralSettings.BingMapApiKey.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.ThumbnailWidth = this.Settings.GeneralSettings.ThumbnailWidth.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.ThumbnailHeight = this.Settings.GeneralSettings.ThumbnailHeight.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.MapPinSize = this.Settings.GeneralSettings.MapPinSize.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
		}
	}
}
