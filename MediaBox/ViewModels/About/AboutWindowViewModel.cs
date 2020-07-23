
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.About;

namespace SandBeige.MediaBox.ViewModels.About {
	/// <summary>
	/// 概要ウィンドウViewModel
	/// </summary>
	public class AboutWindowViewModel : DialogViewModelBase {

		/// <summary>
		/// カレントライセンス
		/// </summary>
		public IReactiveProperty<License> CurrentLicense {
			get;
		}

		/// <summary>
		/// ライセンステキスト
		/// </summary>
		public IReadOnlyReactiveProperty<string> LicenseText {
			get;
		}

		/// <summary>
		/// ライセンス表記リスト
		/// </summary>
		public ReadOnlyReactiveCollection<License> Licenses {
			get;
		}

		/// <summary>
		/// ウィンドウタイトル
		/// </summary>
		public override string Title {
			get {
				return "概要";
			}
			set {
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public AboutWindowViewModel(AboutModel model) {
			this.Licenses = model.Licenses.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			this.LicenseText = model.LicenseText.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.CurrentLicense = model.CurrentLicense.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
		}
	}
}
