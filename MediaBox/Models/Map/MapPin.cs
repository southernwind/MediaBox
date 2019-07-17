using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Map {
	/// <summary>
	/// マップピン
	/// </summary>
	/// <remarks>
	/// このグループを一つのピンとして表示する
	/// </remarks>
	internal class MapPin : MediaFileCollection {
		/// <summary>
		/// 代表メディア
		/// </summary>
		public IReactiveProperty<IMediaFileModel> Core {
			get;
		} = new ReactivePropertySlim<IMediaFileModel>();

		/// <summary>
		/// 表示領域
		/// </summary>
		/// <remarks>
		/// この領域がかぶるアイテムを吸収していく
		/// </remarks>
		public Rectangle CoreRectangle {
			get;
		}

		/// <summary>
		/// ピン状態
		/// </summary>
		public IReactiveProperty<PinState> PinState {
			get;
		} = new ReactivePropertySlim<PinState>(Composition.Enum.PinState.Unselected);

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="core">代表メディア</param>
		/// <param name="rectangle">表示領域</param>
		public MapPin(IMediaFileModel core, Rectangle rectangle) : base(new ObservableSynchronizedCollection<IMediaFileModel>()) {
			this.Core.Value = core;
			this.Items.Add(core);
			this.CoreRectangle = rectangle;
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Core.Value.FilePath}>";
		}
	}
}
