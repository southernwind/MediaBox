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
	internal class MediaFileInformationsViewModel : ViewModelBase {

		public IReadOnlyReactiveProperty<int> FilesCount {
			get;
		}

		public IReadOnlyReactiveProperty<IEnumerable<IMediaFileViewModel>> Files {
			get;
		}

		/// <summary>
		/// タグリスト
		/// </summary>
		public IReadOnlyReactiveProperty<IEnumerable<ValueCountPair<string>>> Tags {
			get;
		}

		/// <summary>
		/// 代表メディア
		/// </summary>
		public IReadOnlyReactiveProperty<IMediaFileViewModel> RepresentativeMediaFile {
			get;
		}

		/// <summary>
		/// プロパティ
		/// </summary>
		public IReadOnlyReactiveProperty<IEnumerable<MediaFileProperty>> Properties {
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

		public MediaFileInformationsViewModel(MediaFileInformations model) {
			this.Files = model.Files.Select(x => x.Select(this.ViewModelFactory.Create)).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.FilesCount = model.FilesCount.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.RepresentativeMediaFile = model.RepresentativeMediaFile.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Properties = model.Properties.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
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
