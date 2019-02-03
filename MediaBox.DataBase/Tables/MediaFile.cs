﻿using System;
using System.Collections.Generic;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// メディアファイルテーブル
	/// </summary>
	public class MediaFile {
		/// <summary>
		/// メディアファイルID
		/// </summary>
		public long MediaFileId {
			get;
			set;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public string FilePath {
			get;
			set;
		}

		/// <summary>
		/// サムネイルファイル名
		/// </summary>
		public string ThumbnailFileName {
			get;
			set;
		}

		/// <summary>
		/// 緯度
		/// </summary>
		public double? Latitude {
			get;
			set;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public double? Longitude {
			get;
			set;
		}

		/// <summary>
		/// 作成日時
		/// </summary>
		public DateTime CreationTime {
			get;
			set;
		}

		/// <summary>
		/// 編集日時
		/// </summary>
		public DateTime ModifiedTime {
			get;
			set;
		}

		/// <summary>
		/// 最終アクセス日時
		/// </summary>
		public DateTime LastAccessTime {
			get;
			set;
		}

		/// <summary>
		/// ファイルサイズ
		/// </summary>
		public long? FileSize {
			get;
			set;
		}

		/// <summary>
		/// 評価
		/// </summary>
		public int Rate {
			get;
			set;
		}

		/// <summary>
		/// 解像度 幅
		/// </summary>
		public int Width {
			get;
			set;
		}

		/// <summary>
		/// 解像度 高さ
		/// </summary>
		public int Height {
			get;
			set;
		}

		/// <summary>
		/// このメディアファイルを含んでいるアルバムリスト
		/// </summary>
		public virtual ICollection<AlbumMediaFile> AlbumMediaFiles {
			get;
			set;
		}

		/// <summary>
		/// タグリスト
		/// </summary>
		public virtual ICollection<MediaFileTag> MediaFileTags {
			get;
			set;
		}

		/// <summary>
		/// 動画ファイル
		/// </summary>
		public virtual VideoFile VideoFile {
			get;
			set;
		}

		/// <summary>
		/// 画像ファイル
		/// </summary>
		public virtual ImageFile ImageFile {
			get;
			set;
		}
	}
}
