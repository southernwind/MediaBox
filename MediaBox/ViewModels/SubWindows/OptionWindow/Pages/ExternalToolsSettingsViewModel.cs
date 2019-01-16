using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow.Pages {
	internal class ExternalToolsSettingsViewModel : ViewModelBase, ISettingsViewModel {
		public string Name {
			get;
		}

		/// <summary>
		/// 外部ツールリスト
		/// </summary>
		public ReadOnlyReactiveCollection<ExternalTool> ExternalTools {
			get;
		}

		/// <summary>
		/// 選択中外部ツール
		/// </summary>
		public ReactivePropertySlim<ExternalTool> SelectedExternalTool {
			get;
		} = new ReactivePropertySlim<ExternalTool>();

		public ReactiveCollection<string> CanditateExtensions {
			get;
		}

		/// <summary>
		/// 外部ツール追加
		/// </summary>
		public ReactiveCommand AddExternalToolCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 外部ツール削除
		/// </summary>
		public ReactiveCommand<ExternalTool> DeleteExternalToolCommand {
			get;
		} = new ReactiveCommand<ExternalTool>();

		public ExternalToolsSettingsViewModel() {
			this.Name = "外部ツール";
			this.ExternalTools = this.Settings.GeneralSettings.ExternalTools.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			this.AddExternalToolCommand.Subscribe(_ => {
				this.Settings.GeneralSettings.ExternalTools.Add(Get.Instance<ExternalTool>());
			});
			this.DeleteExternalToolCommand.Subscribe(x => {
				this.Settings.GeneralSettings.ExternalTools.Remove(x);
			});
		}
	}
}
