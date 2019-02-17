using System;
using System.Linq;

using Livet;

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
		/// VM作成中にコレクションが変化する可能性がある場合は必ずSyncRootでロックすること。
		/// </summary>
		public ObservableSynchronizedCollection<IMediaFileModel> Items {
			get;
		} = new ObservableSynchronizedCollection<IMediaFileModel>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileCollection() {
			this.Items
				.ToCollectionChanged<IMediaFileModel>()
				.Subscribe(_ => {
					this.Count.Value = this.Items.Count;
				}).AddTo(this.CompositeDisposable);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Items.FirstOrDefault()} ({this.Count.Value})>";
		}
	}
}
