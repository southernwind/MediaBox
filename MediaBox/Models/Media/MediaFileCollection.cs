﻿using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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
		public ReactiveCollection<MediaFileModel> Items {
			get;
		} = new ReactiveCollection<MediaFileModel>();

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
