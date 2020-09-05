using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Map {
	/// <summary>
	/// マップポインター
	/// </summary>
	public class MapPointer : MediaFileCollection, IMapPointer {
		/// <summary>
		/// 代表メディア
		/// </summary>
		public IReactiveProperty<IMediaFileModel> Core {
			get;
		} = new ReactivePropertySlim<IMediaFileModel>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="core">代表メディア</param>
		/// <param name="rectangle">表示領域</param>
		public MapPointer(IMediaFileModel core) : base(new ObservableSynchronizedCollection<IMediaFileModel>()) {
			this.Items.Add(core);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Core.Value.FilePath}>";
		}
	}
}
