using System;
using System.Collections.Generic;
using System.ComponentModel;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album;
using SandBeige.MediaBox.Composition.Interfaces.Models.Gesture;

namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IAlbumViewModel : INotifyPropertyChanged, IDisposable {

		ReadOnlyReactiveCollection<IMediaFileViewModel> Items {
			get;
		}

		IAlbumModel AlbumModel {
			get;
		}

		/// <summary>
		/// 操作受信
		/// </summary>
		IGestureReceiver GestureReceiver {
			get;
		}

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		IReadOnlyReactiveProperty<string> Title {
			get;
		}

		/// <summary>
		/// アルバム読み込み時間(ms)
		/// </summary>
		IReadOnlyReactiveProperty<long> ResponseTime {
			get;
		}

		/// <summary>
		/// フィルタリング前件数
		/// </summary>
		IReadOnlyReactiveProperty<int> BeforeFilteringCount {
			get;
		}

		/// <summary>
		/// 選択中メディアファイル
		/// </summary>
		IReactiveProperty<IEnumerable<IMediaFileViewModel>> SelectedMediaFiles {
			get;
		}

		/// <summary>
		/// カレントメディアファイル
		/// </summary>
		IReadOnlyReactiveProperty<IMediaFileViewModel> CurrentItem {
			get;
		}

		/// <summary>
		/// ズームレベル
		/// </summary>
		IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		/// <summary>
		/// ファイル追加コマンド
		/// </summary>
		ReactiveCommand<IEnumerable<IMediaFileViewModel>> AddMediaFileCommand {
			get;
		}

		/// <summary>
		/// ファイル削除コマンド
		/// </summary>
		ReactiveCommand<IEnumerable<IMediaFileViewModel>> RemoveMediaFileCommand {
			get;
		}
	}
}
