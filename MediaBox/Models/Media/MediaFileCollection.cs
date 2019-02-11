using System;
using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルコレクション
	/// </summary>
	/// <remarks>
	/// 件数とファイルリストを持つ
	/// </remarks>
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
		public ReactiveCollection<IMediaFileModel> Items {
			get;
		} = new ReactiveCollection<IMediaFileModel>();

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

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Items.FirstOrDefault()} ({this.Count.Value})>";
		}
	}
}
