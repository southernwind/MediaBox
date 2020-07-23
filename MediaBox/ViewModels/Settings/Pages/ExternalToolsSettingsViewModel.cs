using System;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.ViewModels.Settings.Pages {
	/// <summary>
	/// 外部ツール設定ViewModel
	/// </summary>
	public class ExternalToolsSettingsViewModel : ViewModelBase, ISettingsViewModel {
		/// <summary>
		/// 設定名
		/// </summary>
		public string Name {
			get;
		}

		/// <summary>
		/// 設定候補画像拡張子
		/// </summary>
		public ReadOnlyReactiveCollection<EnabledAndExtensionPair> CandidateImageExtensions {
			get;
		}

		/// <summary>
		/// 設定候補動画拡張子
		/// </summary>
		public ReadOnlyReactiveCollection<EnabledAndExtensionPair> CandidateVideoExtensions {
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

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ExternalToolsSettingsViewModel(ISettings settings) {
			this.Name = "外部ツール";
			// 候補拡張子読み込み
			this.CandidateImageExtensions = settings.GeneralSettings.ImageExtensions.ToReadOnlyReactiveCollection(x => new EnabledAndExtensionPair(x)).AddTo(this.CompositeDisposable);
			this.CandidateVideoExtensions = settings.GeneralSettings.VideoExtensions.ToReadOnlyReactiveCollection(x => new EnabledAndExtensionPair(x)).AddTo(this.CompositeDisposable);

			this.ExternalTools = settings.GeneralSettings.ExternalTools.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			// 選択中外部ツール切り替わりで候補拡張子の選択状態を読み込み
			var loading = false;
			this.SelectedExternalTool.Subscribe(x => {
				loading = true;
				foreach (var ce in this.CandidateImageExtensions.Union(this.CandidateVideoExtensions)) {
					ce.Enabled.Value = x?.TargetExtensions.Contains(ce.Extension.Value) ?? false;
				}
				loading = false;
			});

			// 候補選択拡張子の有効/無効の切り替わりで選択中外部ツールに反映
			this.CandidateImageExtensions.ObserveElementObservableProperty(x => x.Enabled)
				.Merge(this.CandidateVideoExtensions.ObserveElementObservableProperty(x => x.Enabled)).Subscribe(x => {
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

			this.AddExternalToolCommand.Subscribe(_ => {
				settings.GeneralSettings.ExternalTools.Add(new ExternalToolParams());
			});
			this.DeleteExternalToolCommand.Subscribe(x => {
				settings.GeneralSettings.ExternalTools.Remove(x);
			});
		}

		public class EnabledAndExtensionPair {
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
