using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.ViewModels.Settings.Pages;

namespace SandBeige.MediaBox.ViewModels.Settings {
	/// <summary>
	/// 設定ウィンドウViewModel
	/// </summary>
	public class SettingsWindowViewModel : DialogViewModelBase {
		private readonly ISettings _settings;
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
		/// ウィンドウタイトル
		/// </summary>
		public override string Title {
			get {
				return "設定";
			}
			set {
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SettingsWindowViewModel(ISettings settings, ViewModelFactory viewModelFactory) {
			this._settings = settings;
			this.SettingsPages = new ISettingsViewModel[] {
				new GeneralSettingsViewModel(settings).AddTo(this.CompositeDisposable),
				new PathSettingsViewModel().AddTo(this.CompositeDisposable),
				new ExternalToolsSettingsViewModel(settings).AddTo(this.CompositeDisposable),
				new ScanSettingsViewModel(settings).AddTo(this.CompositeDisposable),
				new ViewerSettingsViewModel().AddTo(this.CompositeDisposable),
				new PluginSettingsViewModel(viewModelFactory).AddTo(this.CompositeDisposable)
			};

			this.CurrentSettingsPage.Value = this.SettingsPages.FirstOrDefault();
		}

		protected override void Dispose(bool disposing) {
			this._settings.Save();
			base.Dispose(disposing);
		}
	}
}
