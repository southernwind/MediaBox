using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces.Plugins;
using SandBeige.MediaBox.Models.Plugin;

namespace SandBeige.MediaBox.ViewModels.Plugin {
	/// <summary>
	/// プラグインViewModel
	/// </summary>
	internal class PluginViewModel : ViewModelBase {
		private readonly PluginModel _model;
		/// <summary>
		/// プラグインインスタンス
		/// </summary>
		public IPlugin PluginInstance {
			get {
				return this._model.PluginInstance;
			}
		}

		/// <summary>
		/// プラグインが有効か否か
		/// </summary>
		public IReadOnlyReactiveProperty<bool> IsEnabled {
			get;
		}

		/// <summary>
		/// 有効化コマンド
		/// </summary>
		public ReactiveCommand ToEnableCommand {
			get;
		}

		/// <summary>
		/// 無効化コマンド
		/// </summary>
		public ReactiveCommand ToDisableCommand {
			get;
		}

		/// <summary>
		/// プラグイン
		/// </summary>
		/// <param name="pluginModel"></param>
		public PluginViewModel(PluginModel pluginModel) {
			this._model = pluginModel;
			this.IsEnabled = this._model.IsEnabled.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.ToEnableCommand = this.IsEnabled.Select(x => !x).ToReactiveCommand().AddTo(this.CompositeDisposable);
			this.ToEnableCommand.Subscribe(this._model.ToEnable).AddTo(this.CompositeDisposable);
			this.ToDisableCommand = this.IsEnabled.ToReactiveCommand().AddTo(this.CompositeDisposable);
			this.ToDisableCommand.Subscribe(this._model.ToDisable).AddTo(this.CompositeDisposable);
		}
	}
}
