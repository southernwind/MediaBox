using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Base;

namespace SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow.Pages {
	internal class GeneralSettingsViewModel : ViewModelBase, ISettingsViewModel {
		public string Name {
			get;
		}

		public ReactiveProperty<string> BingMapApiKey {
			get;
		}

		/// <summary>
		/// サムネイル幅
		/// </summary>
		public ReactiveProperty<int> ThumbnailWidth {
			get;
			set;
		}

		/// <summary>
		/// サムネイル高さ
		/// </summary>
		public ReactiveProperty<int> ThumbnailHeight {
			get;
			set;
		}

		public ReactiveProperty<int> MapPinSize {
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
