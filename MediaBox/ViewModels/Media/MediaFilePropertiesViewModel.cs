using System;
using System.Collections.Generic;

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
	internal class MediaFilePropertiesViewModel : MediaFileCollectionViewModel<MediaFileProperties> {

		/// <summary>
		/// タグリスト
		/// </summary>
		public ReadOnlyReactivePropertySlim<IEnumerable<ValueCountPair<string>>> Tags {
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

		public MediaFilePropertiesViewModel(MediaFileProperties model) : base(model) {
			this.Tags = this.Model.Tags.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.AddTagCommand.Subscribe(this.Model.AddTag).AddTo(this.CompositeDisposable);
			this.RemoveTagCommand.Subscribe(this.Model.RemoveTag).AddTo(this.CompositeDisposable);

			this.OpenGpsSelectorWindowCommand.Subscribe(x => {
				using (var vm = Get.Instance<GpsSelectorViewModel>()) {
					vm.SetCandidateMediaFiles(this.Items);
					var message = new TransitionMessage(typeof(Views.Media.GpsSelectorWindow), vm, TransitionMode.Modal);
					this.Messenger.Raise(message);
				}
			});
		}
	}
}
