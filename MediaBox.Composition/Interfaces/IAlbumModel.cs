using System;
using System.Collections.Generic;

using Livet;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IAlbumModel : IDisposable {
		ObservableSynchronizedCollection<IMediaFileModel> Items {
			get;
		}

		/// <summary>
		/// フィルタリング前件数
		/// </summary>
		IReactiveProperty<int> BeforeFilteringCount {
			get;
		}

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		IReactiveProperty<string> Title {
			get;
		}

		/// <summary>
		/// アルバム読み込み時間(ms)
		/// </summary>
		IReactiveProperty<long> ResponseTime {
			get;
		}

		/// <summary>
		/// カレントのメディアファイル(単一)
		/// </summary>
		IReactiveProperty<IMediaFileModel> CurrentMediaFile {
			get;
		}

		/// <summary>
		/// カレントのメディアファイル(複数)
		/// </summary>
		IReactiveProperty<IEnumerable<IMediaFileModel>> CurrentMediaFiles {
			get;
		}

		/// <summary>
		/// 一覧ズームレベル
		/// </summary>
		IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		/// <summary>
		/// 操作受信
		/// </summary>
		IGestureReceiver GestureReceiver {
			get;
		}

		ReadOnlyReactiveCollection<IAlbumViewer> AlbumViewers {
			get;
		}

		void SelectPreviewItem();

		void SelectNextItem();

		/// <summary>
		/// メディアファイルリスト読み込み
		/// </summary>
		void LoadMediaFiles();

		/// <summary>
		/// 事前読み込み
		/// </summary>
		/// <remarks>
		/// 事前読み込みしたい画像リストを受け取って受け取った順番どおりに事前読み込みを行う
		/// Remove
		/// </remarks>
		/// <param name="models">事前読み込みが必要なメディアリスト</param>
		void Prefetch(IEnumerable<IMediaFileModel> models);
	}
}