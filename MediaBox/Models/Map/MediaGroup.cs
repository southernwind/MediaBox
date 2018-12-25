using Reactive.Bindings;

using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Map {
	/// <summary>
	/// マップ用メディアグループ
	/// このグループを一つのピンとして表示する
	/// </summary>
	internal class MediaGroup : MediaFileCollection {
		/// <summary>
		/// 代表メディア
		/// </summary>
		public ReactivePropertySlim<MediaFile> Core {
			get;
		} = new ReactivePropertySlim<MediaFile>();

		/// <summary>
		/// 表示領域
		/// この領域がかぶるアイテムを吸収していく
		/// </summary>
		public Rectangle CoreRectangle {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="core">代表メディア</param>
		/// <param name="rectangle">表示領域</param>
		public MediaGroup(MediaFile core, Rectangle rectangle) {
			this.Core.Value = core;
			this.Items.Add(core);
			this.CoreRectangle = rectangle;
		}
	}
}
