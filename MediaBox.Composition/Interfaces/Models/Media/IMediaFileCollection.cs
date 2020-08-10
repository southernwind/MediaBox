
using Livet;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Media {
	public interface IMediaFileCollection : IModelBase {
		/// <summary>
		/// 件数
		/// </summary>
		public IReactiveProperty<int> Count {
			get;
		}

		/// <summary>
		/// メディアファイルリスト
		/// VM作成中にコレクションが変化する可能性がある場合は必ずSyncRootでロックすること。
		/// </summary>
		public ObservableSynchronizedCollection<IMediaFileModel> Items {
			get;
		}
	}
}
