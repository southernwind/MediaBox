using System;
using System.Collections.Generic;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Controls.Controls.VideoPlayer;

using Unosquare.FFME;

namespace SandBeige.MediaBox.ViewModels.Media.ThumbnailCreator {
	/// <summary>
	/// サムネイル作成ViewModel
	/// </summary>
	internal class ThumbnailCreatorViewModel : ViewModelBase {

		/// <summary>
		/// サムネイル作成対象ファイルリスト
		/// </summary>
		public IEnumerable<VideoFileViewModel> Files {
			get;
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
		public IReadOnlyReactiveProperty<ControlPanelViewModel> ControlPanelViewModel {
			get;
		}

		public IReactiveProperty<MediaElement> MediaElementControl {
			get;
		} = new ReactivePropertySlim<MediaElement>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="files">サムネイル作成対象ファイルリスト</param>
		public ThumbnailCreatorViewModel(IEnumerable<VideoFileViewModel> files) {
			this.Files = files;
			this.ControlPanelViewModel = this.MediaElementControl.Select(x => x == null ? null : new ControlPanelViewModel(x)).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.CurrentVideoFile.Subscribe(x => {
				if (this.ControlPanelViewModel.Value == null) {
					return;
				}
				this.ControlPanelViewModel.Value.Source = this.CurrentVideoFile.Value?.FilePath;
			});
		}
	}
}
