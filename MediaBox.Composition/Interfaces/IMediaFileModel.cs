using System;
using System.ComponentModel;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IMediaFileModel : INotifyPropertyChanged, IDisposable {

		/// <summary>
		/// サムネイルが読み込み済みか
		/// </summary>
		bool ThumbnailCreated {
			get;
			set;
		}

		/// <summary>
		/// ファイル情報取得済みか
		/// </summary>
		bool FileInfoLoaded {
			get;
			set;
		}

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
		long FileSize {
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
		/// 存在するファイルか否か
		/// </summary>
		bool Exists {
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
		/// データベースからプロパティ読み込み
		/// </summary>
		void LoadFromDataBase();

		/// <summary>
		/// データベースからプロパティ読み込み
		/// </summary>
		/// <param name="record">データベースレコード</param>
		void LoadFromDataBase(MediaFile record);

		/// <summary>
		/// プロパティの内容からデータベースレコードを作成
		/// </summary>
		/// <returns>レコード</returns>
		MediaFile CreateDataBaseRecord();

		/// <summary>
		/// プロパティの内容でデータベースレコードを更新
		/// </summary>
		/// <returns>レコード</returns>
		void UpdateDataBaseRecord(MediaFile targetRecord);

		/// <summary>
		/// タグ追加
		/// </summary>
		/// <param name="tag">追加するタグ</param>
		void AddTag(string tag);

		/// <summary>
		/// タグ削除
		/// </summary>
		/// <param name="tag">削除するタグ</param>
		void RemoveTag(string tag);
	}
}
