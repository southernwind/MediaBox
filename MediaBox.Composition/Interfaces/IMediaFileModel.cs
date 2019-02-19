using System;
using System.ComponentModel;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IMediaFileModel : INotifyPropertyChanged, IDisposable {

		/// <summary>
		/// メディアファイルID
		/// </summary>
		long? MediaFileId {
			get;
			set;
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
		/// 拡張子
		/// </summary>
		string Extension {
			get;
		}

		/// <summary>
		/// 作成日時
		/// </summary>
		DateTime CreationTime {
			get;
			set;
		}

		/// <summary>
		/// 編集日時
		/// </summary>
		DateTime ModifiedTime {
			get;
			set;
		}

		/// <summary>
		/// 最終アクセス日時
		/// </summary>
		DateTime LastAccessTime {
			get;
			set;
		}


		/// <summary>
		/// ファイルサイズ
		/// </summary>
		long? FileSize {
			get;
			set;
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
			set;
		}

		/// <summary>
		/// 座標
		/// </summary>
		GpsLocation Location {
			get;
			set;
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
			set;
		}

		/// <summary>
		/// タグリスト
		/// </summary>
		ReactiveCollection<string> Tags {
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
		/// <remarks>
		/// - メタデータグループ1
		///		- メタデータ1(タイトル:値)
		///		- メタデータ2(タイトル:値)
		///		- メタデータ3(タイトル:値)
		/// - メタデータグループ2
		///		-メタデータ1(タイトル:値)
		///		- メタデータ2(タイトル:値)
		/// ...という感じで値が入る
		/// 形式さえ合わせれば具象クラスでどんなデータを入れてもOK
		/// </remarks>
		Attributes<Attributes<string>> Metadata {
			get;
		}

		/// <summary>
		/// もしまだ存在していなければ、サムネイル作成
		/// </summary>
		void CreateThumbnailIfNotExists();

		/// <summary>
		/// サムネイル作成
		/// </summary>
		void CreateThumbnail();

		/// <summary>
		/// まだ読み込まれていなければファイル情報読み込み
		/// </summary>
		void GetFileInfoIfNotLoaded();

		/// <summary>
		/// ファイル情報読み込み
		/// </summary>
		void GetFileInfo();

		/// <summary>
		/// データベースからプロパティ読み込み
		/// </summary>
		void LoadFromDataBase();

		/// <summary>
		/// データベースからプロパティ読み込み
		/// </summary>
		/// <param name="record">データベースレコード</param>
		void LoadFromDataBase(MediaFile record);

		/// <summary>
		/// プロパティの内容をデータベースへ登録
		/// </summary>
		/// <returns>登録したレコード</returns>
		MediaFile RegisterToDataBase();
	}
}
