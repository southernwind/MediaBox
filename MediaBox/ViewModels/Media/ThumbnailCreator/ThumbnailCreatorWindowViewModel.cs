using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Controls.Controls.VideoPlayer;

using Unosquare.FFME;

namespace SandBeige.MediaBox.ViewModels.Media.ThumbnailCreator {
	/// <summary>
	/// サムネイル作成ウィンドウViewModel
	/// </summary>
	public class ThumbnailCreatorWindowViewModel : DialogViewModelBase {
		public static string ParameterNameFiles = nameof(ParameterNameFiles);
		/// <summary>
		/// サムネイル作成対象ファイルリスト
		/// </summary>
		public IEnumerable<VideoFileViewModel>? Files {
			get;
			private set;
		}

		/// <summary>
		/// 現在編集中のファイル
		/// </summary>
		public IReactiveProperty<VideoFileViewModel> CurrentVideoFile {
			get;
		} = new ReactivePropertySlim<VideoFileViewModel>();

		/// <summary>
		/// コントロールパネルViewModel
		/// </summary>
		public IReadOnlyReactiveProperty<ControlPanelViewModel?> ControlPanelViewModel {
			get;
		}

		public IReactiveProperty<MediaElement> MediaElementControl {
			get;
		} = new ReactivePropertySlim<MediaElement>();

		/// <summary>
		/// タイトル
		/// </summary>
		public override string? Title {
			get {
				return "サムネイル作成";
			}
			set {
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ThumbnailCreatorWindowViewModel() {
			this.ControlPanelViewModel = this.MediaElementControl.Select(x => x == null ? null : new ControlPanelViewModel(x)).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.CurrentVideoFile.Subscribe(x => {
				if (this.ControlPanelViewModel.Value == null) {
					return;
				}
				this.ControlPanelViewModel.Value.Source = this.CurrentVideoFile.Value?.FilePath;
			});
		}

		public override void OnDialogOpened(IDialogParameters parameters) {
			// サムネイル作成対象ファイルリストの取得
			this.Files = parameters.GetValue<IEnumerable<VideoFileViewModel>>(ParameterNameFiles);
			this.CurrentVideoFile.Value = this.Files.FirstOrDefault();
		}
	}
}
