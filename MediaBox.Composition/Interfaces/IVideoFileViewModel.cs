using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IVideoFileViewModel {
		/// <summary>
		/// 回転
		/// </summary>
		int? Rotation {
			get;
		}

		/// <summary>
		/// サムネイルイメージ
		/// </summary>
		object Image {
			get;
		}

		/// <summary>
		/// サムネイル作成コマンド
		/// </summary>
		ReactiveCommand<double> CreateThumbnailCommand {
			get;
		}
	}
}
