using System;
using System.Collections.Generic;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Collection;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.ViewModels.Tools;

namespace SandBeige.MediaBox.ViewModels.Media {
	internal interface IMediaFileViewModel {
		/// <summary>
		/// メディアファイルModel
		/// </summary>
		MediaFileModel Model {
			get;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		string FileName {
			get;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		string FilePath {
			get;
		}

		/// <summary>
		/// 対象外部ツール
		/// </summary>
		ReadOnlyReactiveCollection<ExternalToolViewModel> ExternalTools {
			get;
		}

		/// <summary>
		/// サムネイル
		/// </summary>
		Thumbnail Thumbnail {
			get;
		}

		/// <summary>
		/// 座標
		/// </summary>
		GpsLocation Location {
			get;
		}

		/// <summary>
		/// 作成日時
		/// </summary>
		DateTime CreationTime {
			get;
		}

		/// <summary>
		/// 編集日時
		/// </summary>
		DateTime ModifiedTime {
			get;
		}

		/// <summary>
		/// 最終アクセス日時
		/// </summary>
		DateTime LastAccessTime {
			get;
		}

		/// <summary>
		/// ファイルサイズ
		/// </summary>
		long? FileSize {
			get;
		}

		/// <summary>
		/// 評価
		/// </summary>
		int Rate {
			get;
			set;
		}

		/// <summary>
		/// プロパティ
		/// </summary>
		IEnumerable<TitleValuePair<string>> Properties {
			get;
		}
	}
}
