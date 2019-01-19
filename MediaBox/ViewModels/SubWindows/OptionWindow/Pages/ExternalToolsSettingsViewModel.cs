using System;
using System.Linq;

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
		public ReadOnlyReactiveCollection<ExternalToolParams> ExternalTools {
			get;
		}

		/// <summary>
		/// 選択中外部ツール
		/// </summary>
		public IReactiveProperty<ExternalToolParams> SelectedExternalTool {
			get;
		} = new ReactivePropertySlim<ExternalToolParams>();

		public ReadOnlyReactiveCollection<EnabledAndExtensionPair> CanditateExtensions {
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
		public ReactiveCommand<ExternalToolParams> DeleteExternalToolCommand {
			get;
		} = new ReactiveCommand<ExternalToolParams>();

		public ExternalToolsSettingsViewModel() {
			this.Name = "外部ツール";
			this.ExternalTools = this.Settings.GeneralSettings.ExternalTools.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			this.AddExternalToolCommand.Subscribe(_ => {
				this.Settings.GeneralSettings.ExternalTools.Add(Get.Instance<ExternalToolParams>());
			});
			this.DeleteExternalToolCommand.Subscribe(x => {
				this.Settings.GeneralSettings.ExternalTools.Remove(x);
			});

			// 候補拡張子読み込み
			this.CanditateExtensions = this.Settings.GeneralSettings.TargetExtensions.ToReadOnlyReactiveCollection(x => new EnabledAndExtensionPair(x)).AddTo(this.CompositeDisposable);

			// 選択中外部ツール切り替わりで候補拡張子の選択状態を読み込み
			var loading = false;
			this.SelectedExternalTool.Subscribe(x => {
				loading = true;
				foreach (var ce in this.CanditateExtensions.ToArray()) {
					ce.Enabled.Value = x?.TargetExtensions.Contains(ce.Extension.Value) ?? false;
				}
				loading = false;
			});

			// 候補選択拡張子の有効/無効の切り替わりで選択中外部ツールに反映
			this.CanditateExtensions.ObserveElementObservableProperty(x => x.Enabled).Subscribe(x => {
				if (loading) {
					return;
				}
				if (this.SelectedExternalTool.Value == null) {
					return;
				}
				if (x.Value) {
					this.SelectedExternalTool.Value.TargetExtensions.Add(x.Instance.Extension.Value);
				} else {
					this.SelectedExternalTool.Value.TargetExtensions.Remove(x.Instance.Extension.Value);
				}
			});
		}

		internal class EnabledAndExtensionPair {
			public ReactivePropertySlim<bool> Enabled {
				get;
				set;
			} = new ReactivePropertySlim<bool>();

			public ReactivePropertySlim<string> Extension {
				get;
				set;
			} = new ReactivePropertySlim<string>();

			public EnabledAndExtensionPair(string extension) {
				this.Extension.Value = extension;
			}
		}
	}
}
