using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Map;

namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// メディアファイルプロパティ一覧ViewModel
	/// 複数のメディアファイルのプロパティをまとめて一つのプロパティとして閲覧できるようにする
	/// </summary>
	internal class MediaFilePropertiesViewModel : ViewModelBase {

		public ReadOnlyReactivePropertySlim<int> FilesCount {
			get;
		}

		public ReadOnlyReactivePropertySlim<IEnumerable<MediaFileViewModel>> Files {
			get;
		}

		/// <summary>
		/// タグリスト
		/// </summary>
		public ReadOnlyReactivePropertySlim<IEnumerable<ValueCountPair<string>>> Tags {
			get;
		}

		public ReadOnlyReactivePropertySlim<MediaFileViewModel> Single {
			get;
		}

		/// <summary>
		/// タグ追加コマンド
		/// </summary>
		public ReactiveCommand<string> AddTagCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// タグ削除コマンド
		/// </summary>
		public ReactiveCommand<string> RemoveTagCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// GPS設定ウィンドウオープン
		/// </summary>
		public ReactiveCommand OpenGpsSelectorWindowCommand {
			get;
		} = new ReactiveCommand();

		public MediaFilePropertiesViewModel(MediaFileProperties model) {
			this.Files = model.Files.Select(x => x.Select(this.ViewModelFactory.Create)).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.FilesCount = model.FilesCount.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Single = model.Single.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Tags = model.Tags.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.AddTagCommand.Subscribe(model.AddTag).AddTo(this.CompositeDisposable);
			this.RemoveTagCommand.Subscribe(model.RemoveTag).AddTo(this.CompositeDisposable);

			this.OpenGpsSelectorWindowCommand.Subscribe(x => {
				using (var vm = Get.Instance<GpsSelectorViewModel>()) {
					vm.SetCandidateMediaFiles(this.Files.Value);
					var message = new TransitionMessage(typeof(Views.Media.GpsSelectorWindow), vm, TransitionMode.Modal);
					this.Messenger.Raise(message);
				}
			}).AddTo(this.CompositeDisposable);
		}
	}
}
