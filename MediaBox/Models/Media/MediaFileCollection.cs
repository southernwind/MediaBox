using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Base;

namespace SandBeige.MediaBox.Models.Media {
	internal class MediaFileCollection : ModelBase {
		/// <summary>
		/// 件数
		/// </summary>
		public IReactiveProperty<int> Count {
			get;
		} = new ReactivePropertySlim<int>();

		/// <summary>
		/// メディアファイルリスト
		/// </summary>
		public ReactiveCollection<MediaFile> Items {
			get;
		} = new ReactiveCollection<MediaFile>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileCollection() {
			this.Items
				.ToCollectionChanged()
				.Subscribe(_ => {
					this.Count.Value = this.Items.Count;
				}).AddTo(this.CompositeDisposable);
		}
	}
}
