using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.ViewModels.Settings.Pages;

namespace SandBeige.MediaBox.ViewModels.Settings {
	/// <summary>
	/// 設定ウィンドウViewModel
	/// </summary>
	internal class SettingsWindowViewModel : ViewModelBase {
		/// <summary>
		/// 設定ページリスト
		/// </summary>
		public ISettingsViewModel[] SettingsPages {
			get;
		}

		/// <summary>
		/// カレント設定ページ
		/// </summary>
		public IReactiveProperty<ISettingsViewModel> CurrentSettingsPage {
			get;
		} = new ReactiveProperty<ISettingsViewModel>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SettingsWindowViewModel() {
			this.SettingsPages = new ISettingsViewModel[] {
				new GeneralSettingsViewModel().AddTo(this.CompositeDisposable),
				new PathSettingsViewModel().AddTo(this.CompositeDisposable),
				new ExternalToolsSettingsViewModel().AddTo(this.CompositeDisposable),
				new ScanSettingsViewModel().AddTo(this.CompositeDisposable),
				new ViewerSettingsViewModel().AddTo(this.CompositeDisposable)
			};

			this.CurrentSettingsPage.Value = this.SettingsPages.FirstOrDefault();
		}

		protected override void Dispose(bool disposing) {
			this.Settings.Save();
			base.Dispose(disposing);
		}
	}
}
