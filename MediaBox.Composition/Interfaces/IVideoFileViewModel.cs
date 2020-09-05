using System.Collections.Generic;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IVideoFileViewModel : IMediaFileViewModel {
		/// <summary>
		/// 回転
		/// </summary>
		int? Rotation {
			get;
		}

		/// <summary>
		/// サムネイルイメージ
		/// </summary>
		object? Image {
			get;
		}


		/// <summary>
		/// サムネイルファイルリスト
		/// </summary>
		IEnumerable<string> ThumbnailFileList {
			get;
		}

		/// <summary>
		/// サムネイル作成コマンド
		/// </summary>
		ReactiveCommand<double> CreateThumbnailCommand {
			get;
		}

		/// <summary>
		/// 選択中サムネイルインデックス
		/// </summary>
		int SelectedThumbnailIndex {
			get;
			set;
		}
	}
}
