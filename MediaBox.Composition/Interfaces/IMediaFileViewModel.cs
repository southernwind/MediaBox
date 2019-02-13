using System;
using System.ComponentModel;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces {
	/// <summary>
	/// メディアファイルViewModelインターフェイス
	/// </summary>
	/// <remarks>
	/// WPFのViewがジェネリックに対応していないため作成
	/// </remarks>
	public interface IMediaFileViewModel : INotifyPropertyChanged, IDisposable {
		/// <summary>
		/// メディアファイルModel
		/// </summary>
		IMediaFileModel Model {
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
		/// サムネイル
		/// </summary>
		IThumbnail Thumbnail {
			get;
		}

		/// <summary>
		/// 解像度
		/// </summary>
		ComparableSize? Resolution {
			get;
		}

		/// <summary>
		/// 座標
		/// </summary>
		GpsLocation Location {
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
		/// 不正なファイル
		/// </summary>
		bool IsInvalid {
			get;
		}

		/// <summary>
		/// タグリスト
		/// </summary>
		ReadOnlyReactiveCollection<string> Tags {
			get;
		}

		/// <summary>
		/// プロパティ
		/// </summary>
		Attributes<string> Properties {
			get;
		}

		/// <summary>
		/// メディアファイルのメタデータ
		/// </summary>
		Attributes<Attributes<string>> Metadata {
			get;
		}
	}
}
