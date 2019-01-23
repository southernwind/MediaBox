﻿using System;
using System.Collections.Generic;

using Reactive.Bindings;

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
		/// 緯度
		/// </summary>
		double? Latitude {
			get;
		}

		/// <summary>
		/// 経度
		/// </summary>
		double? Longitude {
			get;
		}

		/// <summary>
		/// 日付時刻
		/// </summary>
		DateTime Date {
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
		IEnumerable<TitleValuePair> Properties {
			get;
		}

		/// <summary>
		/// サムネイル再作成コマンド
		/// </summary>
		ReactiveCommand RecreateThumbnailCommand {
			get;
		}
	}
}