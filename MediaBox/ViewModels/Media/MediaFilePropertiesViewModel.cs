using System;
using System.Collections.Generic;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Media {
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

		public MediaFilePropertiesViewModel() : base(Get.Instance<MediaFileProperties>()) {
			this.Tags = this.Model.Tags.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.AddTagCommand.Subscribe(this.Model.AddTag).AddTo(this.CompositeDisposable);
			this.RemoveTagCommand.Subscribe(this.Model.RemoveTag).AddTo(this.CompositeDisposable);
		}
	}
}
