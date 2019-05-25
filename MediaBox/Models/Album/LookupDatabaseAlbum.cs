using System;
using System.Linq;
using System.Linq.Expressions;

using Livet;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// データベース検索アルバム
	/// </summary>
	internal class LookupDatabaseAlbum : AlbumModel {
		/// <summary>
		/// 検索条件 タグ名
		/// </summary>
		public string TagName {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LookupDatabaseAlbum(IFilterSetter filter, ISortSetter sort) : base(new ObservableSynchronizedCollection<IMediaFileModel>(), filter, sort) {
		}

		/// <summary>
		/// データベース読み込み
		/// </summary>
		public void LoadFromDataBase() {
			this.LoadMediaFiles();
		}

		/// <summary>
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected override Expression<Func<MediaFile, bool>> WherePredicate() {
			return mediaFile => mediaFile.MediaFileTags.Select(x => x.Tag.TagName).Contains(this.TagName);
		}
	}
}
